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
            Debug.LogError("[GuestManager] ghostPrefabs가 비어있습니다.");
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
            Debug.LogError("[GuestManager] 풀(당구)이 비어있습니다.");
            return;
        }

        // ✅ 후보 인덱스 생성: 직전 유령(인덱스) 제외
        List<int> candidates = new List<int>();
        for (int i = 0; i < pool.Count; i++)
        {
            if (i == lastGuestId) continue;
            if (pool[i] != null) candidates.Add(i);
        }

        // ✅ 안전장치: 후보가 없다면(풀 1개뿐인 경우 등) 그냥 0번
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
        // 주문 생성(레시피 랜덤)
        List<DrinkRecipe> recipes = GameManager.instance.allRecipes;
        if (recipes == null || recipes.Count == 0)
        {
            Debug.LogError("메뉴판(Recipes)이 비어있습니다! GameManager를 확인하세요.");
            // 주문 생성 실패면 손님을 보내버리고 다음으로 넘김
            evaluateLocked = true;
            EnterEvaluate(submitted: false, madeDrinkName: null);
            return;
        }

        int randomIndex = Random.Range(0, recipes.Count);
        DrinkRecipe selectedMenu = recipes[randomIndex];
        currentOrderName = selectedMenu.drinkName;

        // GameManager에 저장(네 기존 구조 유지)
        GameManager.instance.currentOrderName = currentOrderName;

        // UI
        if (speechBubbleText != null) speechBubbleText.text = currentOrderName;
        if (makeButton != null) makeButton.interactable = true;

        // 인내심 시작
        StartPatience();
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

        // 인내심 종료
        if (patienceRoutine != null)
        {
            StopCoroutine(patienceRoutine);
            patienceRoutine = null;
        }
        if (patienceSlider != null) patienceSlider.gameObject.SetActive(false);

        // 판정
        bool success = false;
        /*
        if (submitted && !string.IsNullOrEmpty(madeDrinkName))
        {
            success = (madeDrinkName == currentOrderName);
        }
        else
        {
            success = false; // 타임아웃/미제출은 실패
        }

        lastResultSuccess = success;
        

        // 성불도/성공횟수 업데이트는 성공 확정 순간(여기)에서만
        lastAscensionUp = false;
        if (success && currentProgress != null)
        {
            lastAscensionUp = currentProgress.OnOrderSuccess();
        }
        */
        // React로 넘어가기
        EnterReact();
    }
    //이후 성불도 로직과 연결
    private void EnterReact()
    {
        state = State.React;

        // React는 "보여주기"만: 데이터 변경 X
        if (speechBubbleText != null)
        {
            if (lastResultSuccess)
            {
                //성불도 상승 및 애니메이터
            }
            else
            {
                //성불도 하강
            }
        }

        // React 끝나면 FlowRoutine에서 leave로 넘어감
    }
    
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