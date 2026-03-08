using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    // 게임매니저에서 레벨, 돈 고정 x -> 데이터 로드 방식으로 변경
    public int level; // 초기 레벨
    public int money; // 초기 돈
    public int currentExp = 0;
    public int maxExp = 100;

    [Header("# 게임 데이터")]
    public List<IngredientData> allIngredients; // 모든 재료 목록
    public DrinkRecipeBook recipebook; //레시피 리스트 호출 : recipebook.allRecipes
    public List<GuestData> allGuests;

    // ★ [추가됨] 현재 주문 중인 손님 정보를 담을 변수
    [Header("# 현재 손님 정보")]
    public GuestData currentGuest;
    public DrinkData currentDrink;
    public string currentOrderName = ""; // 주문한 음료 이름
    public string reactDialogue = ""; // 마지막 주문 결과에 따른 반응 텍스트 저장
    public GameObject SpawnPoint;
    
    [Header("# 던전 인벤토리 연동")]
    public InventoryData playerInventory;

    [Header("# 인내심 로직")]
    public bool orderActive; // 인내심 활성화 여부 (false면 타이머 작동 x)
    public float patienceTotal;
    public float patienceRemaining;

    [Header("# 타이머 플래그")]
    public bool isGamePaused = false; // 게임 전체 일시정지 여부,
    public bool isPaused = false; //도감 이동 코루틴 정지
    public bool isScenePausesd = false;

    [Header("# 반응 플래그")]
    public bool reactPending; // 제조 -> 메인 이동 시 유령 반응 발생 여부
    public bool lastResultSuccess; // 마지막 주문 결과 (성공/실패) 저장
    public bool isAscendMode = false; // 성불 모드 On
    public bool isLevelUpPending = false;

    public static GameManager instance;
    public SnapShot snapshot; // 메인 씬 저장고
    public bool hasSnapshot = false;

    [Header("#던전 데이터")]
    public WeaponInfo weaponInfo;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Data data = MainData.Load(); // 레벨, 돈 데이터 load
        money = data.money;
        level = data.level;
    }
    // 🥨 [중요] 게스트 매니저 데이터 보존
    public void CaptureMainFlow()
    {
        SnapShot shot = new SnapShot();

        // 1. 게스트 매니저의 State 가져오기
        if (GuestManager.instance != null)
        {
            shot.state = GuestManager.instance.state;
        }
        // 1-1. 없으면 처음부터 재시작
        else shot.state = GuestManager.State.Boot; 

        // 2. 게임 매니저 기존 데이터 저장
        shot.currentOrderName = currentOrderName;
        shot.currentGuest = currentGuest;
        shot.currentDrink = currentDrink;
        shot.orderActive = orderActive;
        shot.lastResultSuccess = lastResultSuccess;
        shot.reactDialogue = reactDialogue; 

        // 3. 인내심 저장
        isScenePausesd = true;
        shot.patienceRemaining = patienceRemaining;
        shot.patienceTotal = patienceTotal;
        // 4. 모든 데이터 저장
        snapshot = shot;
        hasSnapshot = true;
    }

    // 데이터 리턴 + 기존 데이터 shot 비우기
    public SnapShot ConsumeSnapshot()
    {
        SnapShot Shot = snapshot;
        snapshot = null;
        hasSnapshot = false;
        return Shot;
    }
    // 돈 더하는 함수
    public static void AddMoney(int amount)
    {
        GameManager.instance.money += amount;
        MainData.Save(GameManager.instance.money, GameManager.instance.level); // 데이터 저장
    }

    //[변경]
    //기존 allRecipe -> recipebook.allRecipes / 기존 DrinkRecipe -> DrinkData
    //음료 이름 -> 레시피 반환 함수
    public DrinkData GetRecipeByName(string searchName)
    {
        foreach (DrinkData recipe in recipebook.allRecipes)
        {
            if (recipe.drinkName == searchName)
                return recipe;
        }
        // 오류 로그는 필요시 주석 해제
        // Debug.LogError("오류: " + searchName + " 레시피를 찾을 수 없습니다.");
        return null;
    }

    // 인내심 관련 로직 게임 매니저로 옮겼습니다
    // 인내심 데이터 관리
    void Update()
    {
        if (!orderActive || isPaused || isScenePausesd) return;

        // 1. 인내심 감소
        patienceRemaining -= Time.deltaTime;

        // 2. 제조 중 인내심 바닥 -> 메인으로 이동
        if (patienceRemaining <= 0f)
        {
            patienceRemaining = 0f;
            OrderTimeout();
        }
    }

    // 메인 -> 제조 화면에서 인내심 데이터 연동
    public void StartOrderTimer(float patienceTime)
    {
        orderActive = true; 
        patienceTotal = patienceTime;
        patienceRemaining = patienceTime; // 제조 직전 인내심 시간
    }
    // 🥨[추가] 제조 -> 메인 화면에서 제조 완료 끝 알림
    public void StopOrderTimer()
    {
        orderActive = false;
    }

    // 🥨[추가] 인내심 시간 계산 로직
    public float GetPatienceNormalized()
    {
        if (!orderActive || patienceTotal <= 0f) return 0f;
        return patienceRemaining / patienceTotal;
    }

    // 🥨[추가] 인내심 시간 초과 시 처리 로직
    void OrderTimeout()
    {
        orderActive = false;
        lastResultSuccess = false;
        reactDialogue = "Time Over!";
        reactPending = true;

        // 🥨 [추가] 인내심 바닥 시 만족도 감소
        // 음료 레벨에 따라 감소량 증가 (예시: 레벨 1 음료 -> -20, 레벨 2 음료 -> -40)
        int DrinkNum = recipebook.allRecipes.IndexOf(currentDrink) + 1;
        int Down = - (DrinkNum * 20); 

        UpdateGuestSatisfaction(currentGuest.guestName, Down);

        // 🥨 [중요] 인내심 바닥 -> 메인 화면으로 강제 이동
        if (SceneManager.GetActiveScene().name == "MakeScene")
        {
            SceneManager.LoadScene("MainScene");
        }
        else // 메인화면에서 인내심 바닥 -> 반응 바로 실행
        {
            StartCoroutine(GuestManager.instance.EnterReact());
        }
    }

    // 🥨[추가] 게임 전체 일시정지 기능
    public void GameIsPaused(bool paused)
    {
        isGamePaused = paused;
    }

    //[🚦추가] 도감 이동 시 일시정지 기능
    public void SetPause(bool pause)
    {
        isPaused = pause;
    }

    // ★ [수정됨] 변수명 변경 반영 (currentSatisfaction 사용)
    public void UpdateGuestSatisfaction(string name, float amount)
    {
        // 리스트에서 이름이 같은 손님 찾기
        GuestData guest = allGuests.Find(g => g.guestName == name);
        // 리스트에 없으면 새로 등록
        if (guest == null)
        {
            guest = new GuestData();
            guest.guestName = name;
            guest.currentSatisfaction = 0; // 초기화
            guest.isAscended = false;
            ProgressManager.instance.SaveGuestProgress(guest);
            allGuests.Add(guest);
        }

        // 🥨 [수정] 만족도 100 달성 -> 증가 x
        guest.currentSatisfaction += amount;
        // 🥨 [추가] 현 만족도를 최소 0, 최대 100 으로 조정 연산
        guest.currentSatisfaction = Mathf.Clamp(guest.currentSatisfaction, 0, guest.maxSatisfaction);
        ProgressManager.instance.SaveGuestProgress(guest);
        Debug.Log($"[{name}] 현재 만족도: {guest.currentSatisfaction} / {guest.maxSatisfaction}");

        // 목표 점수(100) 넘으면 성불
        if (guest.currentSatisfaction >= guest.maxSatisfaction && !isAscendMode)
        {
            isAscendMode = true; // 성불 모드 : 게스트 매니저에서 성불 진행 후 Leave 상태 진입 모드

            // 도감 해금 시점 미룰까
            guest.isAscended = true;
            ProgressManager.instance.SaveGuestProgress(guest);
            Debug.Log($"✨ [{name}] 성불 완료! 도감 해금!");
        }
    }
    public IngredientData GetIngredientData(string name)
    {
        return allIngredients.Find(x => x.ingredientName == name);
    }//이름으로 재료 데이터 찾는 함수
    public void GainExp(int exp)
    {
        currentExp += exp;

        bool isLevelUp = false; // 레벨업 했는지 체크

        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            level++;
            maxExp += 100;
            MainData.Save(GameManager.instance.money, GameManager.instance.level);
            isLevelUp = true; // 레벨업 발생!
        }

        if (isLevelUp)
        {
            Debug.Log($"🎉 레벨업! 현재 레벨: {level}");

            isLevelUpPending = true;
        }
    }
    private Dictionary<string, int> memoryCounts = new Dictionary<string, int>();

    // 데이터 초기화 버튼 연결
    public void ResetGameData()
    {
        MainData.Reset();
        level = 1;
        money = 2500;
        MainData.Save(GameManager.instance.money,GameManager.instance.level);
    }
}

// 🥨 [중요] 게스트매니저 데이터 저장소
[System.Serializable]
public class SnapShot
{
    // GuestManager 진행 상태(재개 지점)
    public GuestManager.State state;

    // 주문/손님 데이터 (GameManager가 이미 들고 있는 값들)
    public string currentOrderName;
    public GuestData currentGuest;
    public DrinkData currentDrink;
    public bool lastResultSuccess;
    public string reactDialogue;

    // 타이머/흐름 플래그
    public bool orderActive;
    public float patienceRemaining;
    public float patienceTotal;
}

// 재료 설계도
[System.Serializable]
public class IngredientData
{
    public string ingredientName;
    public int ingredientCount; // 재료 갯수 변수 설정
    public int unlockLevel;
    public Sprite icon;
    public AudioClip soundEffect;
}


/// <summary>
// DrinkRecipe를 DrinkData로 변경 (DrinkRecipe 사용 x) 
// 게임매니저 인스펙터에서 기존 allRecipe 삭제 후 생성한 Data들 주입
// 기존 List<DrinkRecipe> allRecipe -> DrinkRecipeBook recipebook
// 모든 레시피 호출은 recipebook.allRecipes로 호출, 이렇게 호출한 객체 = List
// 사용 형태: List<DrinkData> recipes = GameManager.instance.recipebook.allRecipes;
//------------------------------------------------------------------------
// 기존 음료 레시피 -> DrinkData로 개별저장
// 호출 시 DrinkData 변수로 호출 
// Drink Data 내부 변수는 기존 클래스 그대로 유지
// 데이터 내부 호출 시 DrinkData.drinkName, Drink.drinkIcon... 이런식
/// </summary>
/*[System.Serializable]
public class DrinkRecipe
{
    public string drinkName;
    public int unlockLevel;
    public string[] requiredIngredients;
    public bool hasMade = false;
    public Sprite drinkIcon;
}*/

// 새로만든 데이터와 기존 이름이 같아 기존 클래스 명을 GuestData_0로 바꾸었습니다
// GuestData_0 는 이제 사용x 
// 손님 데이터 관련 로직 코드들은 전부 기존 변수 그대로 따라서 코드 변경 x
// 게임매니저 인스펙터에서 기존 AllGuest 삭제 후 생성한 Data들 주입
/*[System.Serializable]
public class GuestData_0
{
    public string guestName; // 손님 이름
    public int unlockLevel; // 등장 레벨
    public string orderDrinkName; // 주문할 음료
    public int currentSatisfaction = 0; // 현재 만족도 (0부터 시작)
    public int maxSatisfaction = 100;   // 목표 만족도 (성불 기준, 기본 100)
    
    public bool isAscended = false; // 성불 여부
    public bool hasMet = false;
    public Sprite guestIcon;
    [TextArea]
    public string dialogue; // 대사
}*/
