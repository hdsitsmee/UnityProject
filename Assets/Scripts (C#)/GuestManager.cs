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
        Boot, WaitFirst, Order, React, Leave
    }
    public State state;

    // ===== Runtime =====
    public readonly List<GameObject> pool = new List<GameObject>();
    public GameObject CurrentGuest;

    // 주문 데이터
    public string currentOrderName;

    void Awake()
    {
        instance = this;
        if (spawnPoint == null)
            spawnPoint = transform;

        // UI 기본 정리
        if (OrderBullon != null)
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
        // 0-1. 게임 첫 시작 시 손님 풀 생성
        BuildPool();
        StartCoroutine(StartFlow());
    }
    //🥨 [추가] 반응 로직 -> EnterReact // 성불 실행 -> 성불 실행 끝날 때까지 대기 -> 다음 손님 스폰
    IEnumerator StartFlow()
    {
        if (GameManager.instance != null && GameManager.instance.reactPending)
        {
            GameManager.instance.reactPending = false;
            StartCoroutine(EnterReact());
            yield break;
        }

        else if (GameManager.instance != null && GameManager.instance.isAscendMode)
        {
            StartCoroutine(AscendManager.instance.StartAscend());

            yield return new WaitUntil(() =>
                !GameManager.instance.isAscendMode
            );
        }

        StartFirstGuest();
    }
    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused) return;
        // 🥨 [추가] Order 중 메인,제조 두 씬에서 인내심 표시 갱신
        if (patienceSlider != null && GameManager.instance != null)
        {
            if (GameManager.instance.orderActive) //Order : 인내심 표기
            {
                patienceSlider.gameObject.SetActive(true);
                float normalized = GameManager.instance.GetPatienceNormalized();
                patienceSlider.value = normalized;
            }
            else // Order 아닌 경우 : 인내심 숨김
            {
                patienceSlider.gameObject.SetActive(false);
            }
        }
    }

    // 1. 게임 시작 (Boot) → 첫 손님 대기(WaitFirst)
    private void StartFirstGuest()
    {
        state = State.Boot;
        Debug.Log("게임 시작: Boot");
        ResetUI();
        DeactivateAllGhosts();
        CurrentGuest = null;

        StartCoroutine(FirstGuestRoutine());
    }
    // 2. 첫 손님 대기(WaitFirst) → 유령 등장 및 주문 생성(Order)
    private IEnumerator FirstGuestRoutine()
    {
        state = State.WaitFirst;
        Debug.Log("첫 손님 대기: WaitFirst");
        yield return StartCoroutine(WaitSecondsPaused(firstGuestDelay));
        SpawnEnterOrder();
    }

    private void SpawnEnterOrder()
    {
        SpawnNextGuest();
        BeginOrder();
    }

    // ====== 0~2번까지 과정 함수 ======
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
    // ================================

    //3. Order : 유령 등장 및 주문 생성
    //3-1. 유령 등장
    private void SpawnNextGuest()
    {
        StartCoroutine(WaitWhilePaused());
        state = State.Order;
        Debug.Log("주문 시작: Order");
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
        CurrentGuest = targetObj;
        CurrentGuest.transform.position = spawnPoint.position;
        CurrentGuest.transform.rotation = spawnPoint.rotation;
        CurrentGuest.SetActive(true);
        // 🥨 [추가] 등장 시 얼굴 표정 초기화
        var gv = CurrentGuest.GetComponent<GhostVisual>();
        gv.ShowFace(GhostVisual.Face.Stand); // 표정 초기화


        // 5. [🔥중요] GameManager에 현재 손님 정보 등록 (주문 단계 전에 미리 등록)
        GameManager.instance.currentGuest = selectedData;
        
        // 도감 해금 처리
        if (!selectedData.hasMet)
        {
            selectedData.hasMet = true;
            Debug.Log($"📖 새로운 손님 발견: {selectedData.guestName}");
        }

    }
    //3-2. 주문 생성
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
        GameManager.instance.StartOrderTimer(patienceTime);

        if (patienceSlider != null) 
        {
            patienceSlider.gameObject.SetActive(true);
            patienceSlider.value = 1f; // 초기값은 100%
        }
    }
    //4. React : 주문 결과에 따른 반응 및 퇴장
    public IEnumerator EnterReact()
    {
        // 🥨 [추가] 레벨업 팝업 시 일시정지
        Debug.Log($"반응 대기: {GameManager.instance.isGamePaused}");
        while (GameManager.instance.isGamePaused)
            yield return null;
        yield return StartCoroutine(WaitSecondsPaused(0.2f)); // 일시정지 해제 후 약간의 딜레이

        StartCoroutine(WaitWhilePaused());
        state = State.React;
        Debug.Log("반응 시작: React");

        // 1. 제조 버튼 비활/인내심 비활/현재손님 재등록 및 활성화/말풍선도 ㄱㄱ
        if (makeButton != null) makeButton.interactable = false; //제조 버튼 비활
        if (patienceSlider != null) patienceSlider.gameObject.SetActive(false); //인내심 비활
        if (GameManager.instance.currentGuest != null)//손님 재등록 및 활성화
        {
            if (GameManager.instance != null && GameManager.instance.currentGuest != null)
            {
                //1. GameManager에 현재 손님 정보 가져오기
                GuestData cg = GameManager.instance.currentGuest;
                //2. Pool에서 해당 유령 프리팹 이름과 일치하는 오브젝트 찾기
                GameObject targetObj = null;
                if (cg.ghostPrefab != null)
                {
                    string prefabName = cg.ghostPrefab.name;
                    targetObj = pool.Find(g => g != null && g.name.Contains(prefabName));
                }

                if (targetObj == null && pool.Count > 0) targetObj = pool[0];
                //3. 현재 손님에 재등록 (오브젝트 및 위치,활성화)
                CurrentGuest = targetObj;
                CurrentGuest.transform.position = spawnPoint.position;
                CurrentGuest.transform.rotation = spawnPoint.rotation;
                CurrentGuest.SetActive(true);
                Debug.Log($"현재 손님 재등록: {cg.guestName},{GameManager.instance.lastResultSuccess}");
                //🥨 [추가] 반응에 따른 얼굴 표정 변경
                var gv = CurrentGuest.GetComponent<GhostVisual>();
                if (GameManager.instance.lastResultSuccess)
                    gv.ShowFace(GhostVisual.Face.Happy);
                else gv.ShowFace(GhostVisual.Face.Angry);
            }

        }
        if (OrderBullon != null) OrderBullon.gameObject.SetActive(true); // 말풍선 UI 활성화
        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(true);
            speechBubbleText.text = GameManager.instance.reactText;
        }
        // 타이머 호출
        yield return StartCoroutine(WaitSecondsPaused(reactDuration)); 
        // 2. reactDuration 뒤에 Leave로 이동
        StartCoroutine(LeaveRoutine());
    }

    // 5. Leave : 퇴장 → 다음 손님 대기
    private IEnumerator LeaveRoutine()
    {
        EnterLeave();
        yield return StartCoroutine(WaitSecondsPaused(leaveDuration));
        //FinishLeave();

        // React 예약 해제
        if (GameManager.instance != null) GameManager.instance.reactPending = false;
        
        // 다음 손님 대기 후 스폰
        StartCoroutine(NextGuestDelayRoutine());
    }

    // 5-1. 퇴장 시작 (반응 끝나고 바로)
    private void EnterLeave()
    {
        StartCoroutine(WaitWhilePaused());
        state = State.Leave;
        Debug.Log("퇴장: Leave");
        /*
        // 퇴장 연출 중 UI 정리(말풍선은 leaveDuration 끝까지 남겨도 되고, 지금 지워도 됨)
        if (makeButton != null) 
            makeButton.interactable = false;
        */
        // 주문 데이터 초기화
        currentOrderName = "";
        GameManager.instance.currentOrderName = "";
        if (GameManager.instance != null)
        {
            GameManager.instance.currentOrderName = "";
            GameManager.instance.currentDrink = null;
        }
        // 현재 손님 초기화
        if (CurrentGuest != null)
            CurrentGuest.SetActive(false);

        CurrentGuest = null;
        GameManager.instance.currentGuest = null;

        // UI 정리
        if (OrderBullon != null)
            OrderBullon.gameObject.SetActive(false); // [추가] 말풍선 비활성화
        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(false);
            speechBubbleText.text = "";
        }
        if (patienceSlider != null)
            patienceSlider.gameObject.SetActive(false);

    }
    private IEnumerator NextGuestDelayRoutine()
    {
        yield return StartCoroutine(WaitSecondsPaused(leaveDuration));
        // Leave -> Order
        SpawnEnterOrder();
    }
    //🥨[중요] 타이머 로직
    // 도감 타이머 로직
    IEnumerator WaitWhilePaused()
    {
        //도감 켰을 때 중지
        while (GameManager.instance != null && GameManager.instance.isPaused)
            yield return null;
    }
    // 상태 전환 대기 시간 + 도감 타이머
    IEnumerator WaitSecondsPaused(float seconds)
    {
        //기존 코루틴 대기시간 흐르는 중 도감 켰을 때 중지
        float t = 0f;
        while (t < seconds)
        {
            while (GameManager.instance != null && GameManager.instance.isPaused)
                yield return null;

            t += Time.deltaTime;
            yield return null;
        }
    }
}