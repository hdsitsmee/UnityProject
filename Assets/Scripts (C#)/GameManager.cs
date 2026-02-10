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
    public List<DrinkRecipe> allRecipes; // 모든 음료 레시피 목록
    public List<GuestData> allGuests; // 모든 손님 목록
    
    // ★ [추가됨] 현재 주문 중인 손님 정보를 담을 변수
    public GuestData currentGuest; 
    public string currentOrderName = ""; // 주문한 음료 이름
    
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

    // 음료 이름 -> 레시피 반환 함수
    public DrinkRecipe GetRecipeByName(string searchName)
    {
        foreach (DrinkRecipe recipe in allRecipes)
        {
            if (recipe.drinkName == searchName)
            {
                return recipe;
            }
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

// 음료 레시피
[System.Serializable]
public class DrinkRecipe
{
    public string drinkName;
    public int unlockLevel;
    public string[] requiredIngredients;
    public bool hasMade = false;
}

// ★ [수정됨] 손님 설계도 (변수 분리)
[System.Serializable]
public class GuestData
{
    public string guestName; // 손님 이름
    public int unlockLevel; // 등장 레벨
    public string orderDrinkName; // 주문할 음료
    
    // ★ 헷갈리지 않게 분리했습니다.
    public int currentSatisfaction = 0; // 현재 만족도 (0부터 시작)
    public int maxSatisfaction = 100;   // 목표 만족도 (성불 기준, 기본 100)
    
    public bool isAscended = false; // 성불 여부
    [TextArea]
    public string dialogue; // 대사
}