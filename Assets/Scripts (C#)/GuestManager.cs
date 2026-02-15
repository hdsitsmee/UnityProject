using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GuestManager : MonoBehaviour
{
    public static GuestManager instance;
   
    [Header("UI")]
    public TMP_Text speechBubbleText;
    public GameObject OrderBullon; // [추가] 말풍선 UI
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
    public float reactDuration = 2.5f;
    public float leaveDuration = 0.6f;

    [Header("Patience")]
    public float patienceTime = 10f;

    //상태 열거 : 게임 시작, 첫손님 대기 3초, 유령 랜덤 선택, 유령 등장(=활성화), 주문 생성(인내심 생성), 유령 데이터 업뎃, 반응 (성공,실패), 퇴장(=비활성화), 다음손님 대기 3초
    public enum State
    {
        Boot, WaitFirst, Spawn, Arrive, Order, Evaluate, React, Leave, Cooldown
    }
    public State state;

    // ===== Runtime =====
    public readonly List<GameObject> pool = new List<GameObject>();
    public GameObject currentGuest;
    //private GhostProgress currentProgress; //성불도 클래스 및 변수

    //직전 출현 유령id
    //private int lastGuestId = -1;

    private Coroutine patienceRoutine;
    private Coroutine flowRoutine;
    public bool isPaused; //[🚦추가] 도감 이동 코루틴 정지
    private bool evaluateLocked; // 한 손님당 Evaluate 1회 보장

    // 주문 데이터
    public string currentOrderName;

    // 결과 데이터(React에서 사용)
    private bool lastResultSuccess;
    private bool lastAscensionUp;

    void Awake()
    {
        instance = this; 
        if (spawnPoint == null) 
            spawnPoint = transform;

        // UI 기본 정리
        if (OrderBullon != null) // [추가] 말풍선 UI 비활성화
            OrderBullon.gameObject.SetActive(false);
        if (makeButton != null) 
            makeButton.interactable = false;
        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(false);
            speechBubbleText.text = "";
        }
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
    //[🚦추가] 도감 이동 시 일시정지 기능
    public void SetPause(bool pause)
    {
        isPaused = pause;
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
        Debug.Log("State: Boot");

        ResetUI();
        DeactivateAllGhosts();
        currentGuest = null;
        //lastGuestId = -1;

        // WAIT_FIRST 첫손님 대기 3초
        state = State.WaitFirst;
        Debug.Log("State: WaitFirst");
        yield return new WaitForSeconds(firstGuestDelay);

        // 2번째 손님부터는 계속 다음 손님 이후 3초 텀이므로 while에서 로직 진행
        while (true)
        {
            // SPAWN 유령 랜덤 선택 및 등장(=활성화)
            if (isPaused)
            {
                yield return null;
                continue;
            }
            state = State.Spawn;
            Debug.Log("State: Spawn");
            evaluateLocked = false;
            SpawnNextGuest();

            // ARRIVE 유령 등장
            state = State.Arrive;
            Debug.Log("State: Arrive");
            yield return new WaitForSeconds(arriveDuration);

            // ORDER 주문 생성(인내심 생성)
            state = State.Order;
            Debug.Log("State: Order");
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
            else if (state == State.Leave)
            {
                yield return new WaitForSeconds(leaveDuration);
                FinishLeave();
            }

            // COOLDOWN 다음손님 대기 3초
            else
            {
                state = State.Cooldown;
                yield return new WaitForSeconds(nextGuestDelay);
            }

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

    // Boot : UI정리 및 유령 프리펩 모두 비활성화
    private void ResetUI()
    {
        if (OrderBullon != null) OrderBullon.gameObject.SetActive(false); // [추가] 말풍선 비활성화
        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(true);
            speechBubbleText.text = "";
        }
        if (makeButton != null) makeButton.interactable = false;
        if (patienceSlider != null) patienceSlider.gameObject.SetActive(false);
    }

    private void DeactivateAllGhosts()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null) 
                pool[i].SetActive(false);
        }
    }

    //Spawn : 랜덤 출현 로직
    private void SpawnNextGuest()
    {
       if (pool.Count == 0) return;

        // 1. 현재 레벨에 등장 가능한 'GuestData' 후보군 뽑기
        List<GuestData> candidates = new List<GuestData>();
        int myLevel = GameManager.level;

        foreach (var guest in GameManager.instance.allGuests)
        {
            if (guest.unlockLevel <= myLevel)
            {
                candidates.Add(guest);
            }
        }

        // 안전장치: 없으면 에러 안나게 아무거나 혹은 리턴
        if (candidates.Count == 0)
        {
            Debug.LogError("현재 레벨에 등장 가능한 유령 데이터가 없습니다!");
            return;
        }

        // 2. 후보 중 하나 랜덤 선택 (GuestData)
        GuestData selectedData = candidates[Random.Range(0, candidates.Count)];

        // 3. 선택된 Data에 맞는 유령 오브젝트를 'Pool'에서 찾기
        // (GuestData의 ghostPrefab 이름과 Pool에 있는 오브젝트 이름이 포함관계인지 확인)
        GameObject targetObj = null;
        if (selectedData.ghostPrefab != null)
        {
            string prefabName = selectedData.ghostPrefab.name;
            targetObj = pool.Find(g => g.name.Contains(prefabName));
        }

        // 못 찾았으면 임시로 0번 (에러 방지)
        if (targetObj == null) targetObj = pool[0];

        // 4. 활성화
        currentGuest = targetObj;
        currentGuest.transform.position = spawnPoint.position;
        currentGuest.transform.rotation = spawnPoint.rotation;
        currentGuest.SetActive(true);

        // 5. [🔥중요] GameManager에 현재 손님 정보 등록 (주문 단계 전에 미리 등록)
        GameManager.instance.currentGuest = selectedData;
        
        // 도감 해금 처리
        if (!selectedData.hasMet)
        {
            selectedData.hasMet = true;
            Debug.Log($"📖 새로운 손님 발견: {selectedData.guestName}");
        }

    }

    // =========================
    // Order / Evaluate / React / Leave
    // =========================

   private void BeginOrder()
   {
       // 1. 현재 레벨에 주문 가능한 'DrinkData' 후보군 뽑기
        List<DrinkData> possibleDrinks = new List<DrinkData>();
        int myLevel = GameManager.level;

        foreach (var drink in GameManager.instance.recipebook.allRecipes)
        {
            if (drink.unlockLevel <= myLevel)
            {
                possibleDrinks.Add(drink);
            }
        }

        // 안전장치
        if (possibleDrinks.Count == 0)
        {
            Debug.LogError("주문 가능한 음료가 없습니다!");
            evaluateLocked = true; // 강제 실패 처리
            return;
        }

        // 2. 랜덤 선택
        DrinkData selectedMenu = possibleDrinks[Random.Range(0, possibleDrinks.Count)];
        currentOrderName = selectedMenu.drinkName;

        // 3. GameManager 업데이트
        GameManager.instance.currentDrink = selectedMenu;
        GameManager.instance.currentOrderName = currentOrderName;

        // [🔥참고] 손님 데이터 등록 로직은 SpawnNextGuest로 이동했습니다.
        // 여기서 중복으로 할 필요 없음.

        // 4. UI 업데이트
        if (OrderBullon != null) OrderBullon.gameObject.SetActive(true); 
        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(true);
            speechBubbleText.text = currentOrderName;
        }
        if (makeButton != null) makeButton.interactable = true;

        // 5. 인내심 시작
        StartPatience();
   }
    //인내심 로직
   private void StartPatience()
   {
       Debug.Log($"[StartPatience] called. state={state}, timeScale={Time.timeScale}, patienceTime={patienceTime}");

       if (patienceRoutine != null)
       {
           //Debug.Log("[StartPatience] stop previous routine");
           StopCoroutine(patienceRoutine);
       }

       if (patienceSlider == null)
       {
           //Debug.LogError("[StartPatience] patienceSlider is NULL");
           return;
       }

       patienceSlider.value = 1f;
       patienceSlider.gameObject.SetActive(true);
       //Debug.Log($"[StartPatience] slider activeInHierarchy={patienceSlider.gameObject.activeInHierarchy}, value={patienceSlider.value}");

       patienceRoutine = StartCoroutine(PatienceRoutine());
   }

    private IEnumerator PatienceRoutine()
    {
        Debug.Log($"[PatienceRoutine] start frame. state={state}");

        float t = 0f;
        while (t < patienceTime)
        {
            if (state != State.Order)
            {
                //Debug.LogWarning($"[PatienceRoutine] yield break! state={state}");
                yield break;
            }
            // [🚦추가] 도감 이동 시 일시정지 기능
            if (isPaused)
            {
                yield return null;
                continue;
            }
            t += Time.deltaTime;
            float normalized = 1f - (t / patienceTime);
            patienceSlider.value = normalized;

            yield return null;
        }

        //Debug.Log("[PatienceRoutine] timeout reached");

        if (state == State.Order && !evaluateLocked)
        {
            evaluateLocked = true;
            EnterEvaluate(submitted: false, madeDrinkName: null);
        }
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
            GameManager.instance.GainExp(10); 

            //현재 손님(currentGuest)에게 점수 반영
            if (GameManager.instance.currentGuest != null)
            {
                string guestID = GameManager.instance.currentGuest.guestName;
                GameManager.instance.UpdateGuestSatisfaction(guestID, 34); 
            }
        }
        else
        {
            Debug.Log("제조 실패...");
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
                speechBubbleText.gameObject.SetActive(true);
                speechBubbleText.text = "맛있어! (성불 수치 UP)";
                // 여기에 하트 이모티콘이나 성공 효과음 재생 코드 추가 가능
            }
            else
            {
                speechBubbleText.gameObject.SetActive(true);
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
        if (GameManager.instance != null)
        {
            GameManager.instance.currentOrderName = "";
            GameManager.instance.currentDrink = null; // 이것도 비워주는 게 안전함
        }
    }

    //성불도 로직 구현 시 여기에 성불도 호출
    private void FinishLeave()
    {
        // 유령 비활성화
        if (currentGuest != null)
            currentGuest.SetActive(false);

        currentGuest = null;
        GameManager.instance.currentGuest = null;

        // UI 정리
        if (OrderBullon !=  null)
            OrderBullon.gameObject.SetActive(false); // [추가] 말풍선 비활성화
        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(false);
            speechBubbleText.text = "";
        }
        if (patienceSlider != null) 
            patienceSlider.gameObject.SetActive(false);

        // 다음 루프에서 Cooldown → Spawn
    }
}