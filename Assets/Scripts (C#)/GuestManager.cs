using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GuestManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text speechBubbleText;
    public Button makeButton;
    public Slider patienceSlider;

    [Header("Ghost Pool (5 prefabs)")]
    [Tooltip("유령 프리팹 5개(또는 5개 오브젝트). Start에서 풀로 미리 생성")]
    public GameObject[] ghostPrefabs; // 길이 5 추천
    public Transform spawnPoint;

    [Header("Timing")]
    public float firstGuestDelay = 3f; // 게임 시작 후 첫 손님
    public float nextGuestDelay = 3f;  // 퇴장 후 다음 손님
    public float arriveDuration = 0.5f;
    public float reactDuration = 1.2f;
    public float leaveDuration = 0.6f;

    [Header("Patience")]
    public float patienceTime = 10f;

    //상태 열거 : 게임 시작, 첫손님 대기 3초, 유령 랜덤 선택, 유령 등장(=활성화), 주문 생성(인내심 생성), 유령 데이터 업뎃, 반응 (성공,실패), 퇴장(=비활성화), 다음손님 대기 3초
    private enum State
    {
        Boot, WaitFirst, Spawn, Arrive, Order, Evaluate, React, Leave, Cooldown
    }
    private State state;

    // ===== Runtime =====
    private readonly List<GameObject> pool = new List<GameObject>();
    private GameObject currentGuest;
    //private GhostProgress currentProgress; //성불도 클래스 및 변수

    //직전 출현 유령id
    private int lastGuestId = -1;

    private Coroutine patienceRoutine;
    private Coroutine flowRoutine;

    private bool evaluateLocked; // 한 손님당 Evaluate 1회 보장

    // 주문 데이터
    private string currentOrderName;

    // 결과 데이터(React에서 사용)
    private bool lastResultSuccess;
    private bool lastAscensionUp;

    void Awake()
    {
        if (spawnPoint == null) 
            spawnPoint = transform;

        // UI 기본 정리
        if (makeButton != null) 
            makeButton.interactable = false;
        if (speechBubbleText != null) 
            speechBubbleText.text = "";
        if (patienceSlider != null) //인내심 게이지
            patienceSlider.gameObject.SetActive(false);
    }

    void Start()
    {
        BuildPool();
        StartFlow();
    }

    void OnDisable() //오브젝트 비활성화 시 호출
    {
        StopAllCoroutines(); //유령 퇴장(=비활성화) -> 코루틴 중단 (다음 유령 
    }

    // 플레이어가 음료를 완성/제출했을 때 호출.
    // madeDrinkName: 플레이어가 만든 음료 이름
    public void SubmitDrink(string madeDrinkName)
    {
        if (state != State.Order) 
            return;
        if (evaluateLocked) 
            return;

        evaluateLocked = true;
        EnterEvaluate(submitted: true, madeDrinkName: madeDrinkName);
    }

    // 게임 시작 (=코루틴 시작)
    private void StartFlow()
    {
        if (flowRoutine != null) 
            StopCoroutine(flowRoutine);
        flowRoutine = StartCoroutine(FlowRoutine());
    }

    private IEnumerator FlowRoutine()
    {
        // BOOT 게임 시작 
        state = State.Boot;
        ResetUI();
        DeactivateAllGhosts();
        currentGuest = null;
        lastGuestId = -1;

        // WAIT_FIRST 첫손님 대기 3초
        state = State.WaitFirst;
        yield return new WaitForSeconds(firstGuestDelay);

        // 2번째 손님부터는 계속 다음 손님 이후 3초 텀이므로 while에서 로직 진행
        while (true)
        {
            // SPAWN 유령 랜덤 선택
            state = State.Spawn;
            SpawnNextGuest();

            // ARRIVE 유령 등장(=활성화)
            state = State.Arrive;
            yield return new WaitForSeconds(arriveDuration);

            // ORDER 주문 생성(인내심 생성)
            state = State.Order;
            BeginOrder();

            // ORDER 상태는 (1) SubmitDrink 호출 or (2) 인내심 타임아웃에서 Evaluate (유령 데이터 업뎃) 로 넘어감
            // Evaluate로 넘어가면 React/Leave/Cooldown을 여기서 이어서 진행
            while (state == State.Order)
                yield return null;

            // REACT 반응 (성공,실패)
            if (state == State.React)
            {
                yield return new WaitForSeconds(reactDuration);
                EnterLeave();
            }

            // LEAVE 퇴장(=비활성화)
            if (state == State.Leave)
            {
                yield return new WaitForSeconds(leaveDuration);
                FinishLeave();
            }

            // COOLDOWN 다음손님 대기 3초
            state = State.Cooldown;
            yield return new WaitForSeconds(nextGuestDelay);

            // 다음 루프: Spawn
        }
    }

    // Start : 풀 생성
    private void BuildPool()
    {
        pool.Clear();

        if (ghostPrefabs == null || ghostPrefabs.Length == 0)
        {
            //Debug.LogError("[GuestManager] ghostPrefabs가 비어있습니다.");
            return;
        }

        for (int i = 0; i < ghostPrefabs.Length; i++)
        {
            GameObject prefab = ghostPrefabs[i];
            if (prefab == null) continue;

            GameObject go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            go.SetActive(false);
            pool.Add(go);
        }
    }

    // Boot : 유령 프리펩 모두 비활성화
    private void DeactivateAllGhosts()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null) 
                pool[i].SetActive(false);
        }
    }

    //랜덤 출현 로직
    private void SpawnNextGuest()
    {
        if (pool.Count == 0)
        {
            //Debug.LogError("[GuestManager] 풀(당구)이 비어있습니다.");
            return;
        }

        List<int> candidates = new List<int>();
        for (int i = 0; i < pool.Count; i++)
        {
            if (i == lastGuestId) continue;
            if (pool[i] != null) candidates.Add(i);
        }

        if (candidates.Count == 0)
            candidates.Add(0);

        int picked = candidates[Random.Range(0, candidates.Count)];
        lastGuestId = picked;

        currentGuest = pool[picked];
        currentGuest.transform.position = spawnPoint.position;
        currentGuest.transform.rotation = spawnPoint.rotation;
        currentGuest.SetActive(true);

    }

    // =========================
    // Order / Evaluate / React / Leave
    // =========================

   private void BeginOrder()
    {
        // 1. 주문 생성 (레시피 랜덤 선택)
        List<DrinkRecipe> recipes = GameManager.instance.allRecipes;
        
        // 안전 장치: 레시피가 없으면 에러 방지
        if (recipes == null || recipes.Count == 0)
        {
            Debug.LogError("메뉴판(Recipes)이 비어있습니다! GameManager를 확인하세요.");
            // 주문 실패 처리 후 넘어감
            evaluateLocked = true;
            EnterEvaluate(submitted: false, madeDrinkName: null);
            return;
        }

        // 랜덤 메뉴 선택
        int randomIndex = Random.Range(0, recipes.Count);
        DrinkRecipe selectedMenu = recipes[randomIndex];
        currentOrderName = selectedMenu.drinkName;

        // 2. GameManager에 주문 정보 저장 (MakeManager가 알 수 있게)
        GameManager.instance.currentOrderName = currentOrderName;


        // 3. ★ [핵심 추가] GameManager에 '현재 손님(currentGuest)' 정보 등록
        if (currentGuest != null)
        {
            // 유령 오브젝트 이름에서 "(Clone)" 글자 제거 (예: "Ghost_Girl(Clone)" -> "Ghost_Girl")
            // 공백 제거(.Trim)까지 해서 깔끔한 ID 생성
            string guestID = currentGuest.name.Replace("(Clone)", "").Trim();

            // GameManager의 전체 손님 명부에서 이 이름(ID)을 가진 데이터를 찾음
            GuestData data = GameManager.instance.allGuests.Find(g => g.guestName == guestID);

            // 만약 처음 등장한 손님이라 데이터가 없다면? -> 새로 만들어서 등록!
            if (data == null)
            {
                data = new GuestData();
                data.guestName = guestID;
                data.maxSatisfaction = 100; // 성불 목표치 (기본 100)
                data.currentSatisfaction = 0; // 현재 만족도 0
                data.isAscended = false;
                
                // 명부에 추가
                GameManager.instance.allGuests.Add(data);
                Debug.Log($"새로운 손님 데이터 생성: {guestID}");
            }

            // ★ GameManager에게 "지금 와있는 손님이 이 사람이야!"라고 알려줌
            GameManager.instance.currentGuest = data;
        }


        // 4. UI 업데이트 (말풍선, 버튼 활성화)
        if (speechBubbleText != null) speechBubbleText.text = currentOrderName;
        if (makeButton != null) makeButton.interactable = true;

        // 5. 인내심 타이머 시작
        StartPatience();
        
        // (참고) 튜토리얼 로직은 MakeScene으로 넘어갔을 때 MakeManager가 
        // GameManager.currentOrderName을 보고 알아서 판단하므로 여기선 호출 안 해도 됩니다.
    }
    //인내심 로직
    private void StartPatience()
    {
        if (patienceRoutine != null) 
            StopCoroutine(patienceRoutine);
        if (patienceSlider == null) 
            return;

        patienceSlider.value = 1f;
        patienceSlider.gameObject.SetActive(true);

        patienceRoutine = StartCoroutine(PatienceRoutine());
    }

    private IEnumerator PatienceRoutine()
    {
        float t = 0f;

        while (t < patienceTime)
        {
            // ORDER 상태가 아니면 종료
            if (state != State.Order) yield break;

            t += Time.deltaTime;
            float normalized = 1f - (t / patienceTime);
            patienceSlider.value = normalized;

            yield return null;
        }

        // 타임아웃 → Evaluate(자동 실패)
        if (state == State.Order && !evaluateLocked)
        {
            evaluateLocked = true;
            EnterEvaluate(submitted: false, madeDrinkName: null);
        }
    }
    //이후에 MakeManager와 연결해서 성공 판정 여부 저장
    private void EnterEvaluate(bool submitted, string madeDrinkName)
    {
        state = State.Evaluate;

        // 1. 인내심 타이머 정지 및 숨기기
        if (patienceRoutine != null)
        {
            StopCoroutine(patienceRoutine);
            patienceRoutine = null;
        }
        if (patienceSlider != null) patienceSlider.gameObject.SetActive(false);

        // 2. 성공 여부 판정
        bool success = false;
        
        if (submitted && !string.IsNullOrEmpty(madeDrinkName))
        {
            // 주문한 음료 이름과 만든 음료 이름이 같은지 확인
            success = (madeDrinkName == currentOrderName);
        }
        else
        {
            success = false; // 시간 초과 또는 미제출
        }

        lastResultSuccess = success; // 결과 저장 (React에서 씀)

        // 3. ★ 핵심 로직 추가 (경험치, 성불 수치) ★
        if (success)
        {
            Debug.Log("제조 성공!");
            
            // A. 경험치 획득 (예: 10점)
            GameManager.instance.GainExp(10); 

            // B. 현재 손님의 만족도(성불 수치) 증가 (예: 34점)
            // currentGuest.name은 "Ghost_Girl(Clone)" 처럼 나올 수 있으니 
            // 실제 데이터 ID 관리를 위해선 프리팹 이름이나 별도 ID가 필요하지만, 
            // 일단 화면에 떠있는 유령 이름으로 매칭한다고 가정합니다.
            
            // 주의: 프리팹 이름이 정확히 데이터와 일치해야 함. 
            // 팀원이 만든 프리팹 이름 규칙을 확인 필요. 여기선 currentGuest.name을 사용.
            string guestID = currentGuest.name.Replace("(Clone)", "").Trim(); 
            GameManager.instance.UpdateGuestSatisfaction(guestID, 34); 
        }
        else
        {
            Debug.Log("제조 실패...");
            // 실패 시 패널티가 있다면 여기에 추가
        }

        // 반응 단계로 이동
        EnterReact();
    }
    //이후 성불도 로직과 연결
    private void EnterReact()
    {
        state = State.React;

        if (speechBubbleText != null)
        {
            if (lastResultSuccess)
            {
                speechBubbleText.text = "맛있어! (성불 수치 UP)";
                // 여기에 하트 이모티콘이나 성공 효과음 재생 코드 추가 가능
            }
            else
            {
                speechBubbleText.text = "이게 아니야... (실망)";
                // 여기에 실패 효과음 재생 코드 추가 가능
            }
        }
    }  
        // React 시간이 지나면 FlowRoutine에서 자동으로 Leave(퇴장)로 넘어감
    
    private void EnterLeave()
    {
        state = State.Leave;
        /*
        // 퇴장 연출 중 UI 정리(말풍선은 leaveDuration 끝까지 남겨도 되고, 지금 지워도 됨)
        if (makeButton != null) 
            makeButton.interactable = false;
        */
        // 주문 초기화
        currentOrderName = "";
        GameManager.instance.currentOrderName = "";
    }

    //성불도 로직 구현 시 여기에 성불도 호출
    private void FinishLeave()
    {
        // 유령 비활성화
        if (currentGuest != null)
            currentGuest.SetActive(false);

        currentGuest = null;

        // UI 정리
        if (speechBubbleText != null) 
            speechBubbleText.text = "";
        if (patienceSlider != null) 
            patienceSlider.gameObject.SetActive(false);

        // 다음 루프에서 Cooldown → Spawn
    }

    private void ResetUI()
    {
        if (speechBubbleText != null) speechBubbleText.text = "";
        if (makeButton != null) makeButton.interactable = false;
        if (patienceSlider != null) patienceSlider.gameObject.SetActive(false);
    }
}