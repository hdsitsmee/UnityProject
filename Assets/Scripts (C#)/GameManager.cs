using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static int level = 1; // 초기 레벨
    public static int money = 2500; // 초기 돈
    public int currentExp = 0;
    public int maxExp = 100;
    
    [Header("# 게임 데이터")] 
    public List<IngredientData> allIngredients; // 모든 재료 목록
    public DrinkRecipeBook recipebook; //[🥨변경] 기존 public List<DrinkRecipe> allRecipe, 여기서 레시피 리스트 호출 : recipebook.allRecipes
    public List<GuestData> allGuests; //[🥨변경] 코드 변경 x 데이터 설정을 Assets->data->Guest1,2,3,4...로 옮김

    // ★ [추가됨] 현재 주문 중인 손님 정보를 담을 변수
    public GuestData currentGuest; //[🥨변경] 코드 변경 x 데이터 설정을 Assets->data->Guest1,2,3,4...로 옮김
    public DrinkData currentDrink; //[🥨변경] DrinkRecipe -> DrinkData
    public string currentOrderName = ""; // 주문한 음료 이름
    
    // ★ [추가됨] 인내심 게이지 및 말풍선을 붙이기 위한 위치 변수
    public GameObject SpawnPoint;
    
    public static GameManager instance;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 돈 더하는 함수
    public static void AddMoney(int amount)
    {
        money += amount;
    }

    // 레벨업 함수
    public static void LevelUp()
    {
        level++;
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득! 현재: {currentExp} / {maxExp}");

        // 레벨업 로직
        if (currentExp >= maxExp)
        {
            level++;
            currentExp -= maxExp;
            maxExp += 50; 
            Debug.Log($" Lv.{level}");
        }
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

    // ★ [수정됨] 변수명 변경 반영 (currentSatisfaction 사용)
    public void UpdateGuestSatisfaction(string name, int amount)
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
            allGuests.Add(guest);
        }

        // 만족도 증가
        guest.currentSatisfaction += amount;
        Debug.Log($"[{name}] 현재 만족도: {guest.currentSatisfaction} / {guest.maxSatisfaction}");

        // 목표 점수(100) 넘으면 성불
        if (guest.currentSatisfaction >= guest.maxSatisfaction && !guest.isAscended)
        {
            guest.isAscended = true;
            Debug.Log($"✨ [{name}] 성불 완료! 도감 해금!");
        }
    }
}

// 재료 설계도
[System.Serializable]
public class IngredientData
{
    public string ingredientName;
    public int unlockLevel;
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
