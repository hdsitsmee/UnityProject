using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static int level = 1; //초기 레벨
    public static int money = 2500; //초기 돈
    public static int currentExp = 0;//초기 경험치
    public static int maxExp = 100;//맥스 경험치(초기값 설정)

    public GuestData currentGuest;

    [Header("게임 데이터")]
    public List<IngredientData> allIngredients;//모든 재료 목록
    public List<DrinkRecipe> allRecipes;//모든 음료 레시피 목록
    public List<GuestData> allGuests;//모든 손님 목록
    public string currentOrderName = "";//주문한 음료를 저장할 변수
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

    //돈 더하는 함수
    public static void AddMoney(int amount)
    {
        money += amount;
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log("경험치 획득 +" + amount + " (현재: " + currentExp + "/" + maxExp + ")");

        //경험치가 꽉 찼는지 확인
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
    }
    void LevelUp()//레벨업로직
    {
        currentExp -= maxExp;//남은 경험치는 다음 레벨로 이월
        level++;
        maxExp += 50;// 다음 레벨은 더 많은 경험치가 필요
        
        Debug.Log("레벨업");
    }
    //음료 이름 -> 레시피 함수
    public DrinkRecipe GetRecipeByName(string searchName)
    {
        foreach (DrinkRecipe recipe in allRecipes)
        {
            if (recipe.drinkName == searchName)
            {
                return recipe;
            }
        }
        Debug.LogError("오류: " + searchName + "라는 음료는 레시피 목록에 없습니다! 오타를 확인하세요.");
        return null; 
    }
}

//재료 설계도
[System.Serializable]
public class IngredientData
{
    public string ingredientName; //재료의 이름
    public int unlockLevel;//해금 레벨
}

//음료 레시피
[System.Serializable]
public class DrinkRecipe
{
    public string drinkName;//음료 이름
    public int unlockLevel;//해금 레벨
    public string[] requiredIngredients;//필요한 재료들
    public bool hasMade = false;//만들어 본 적 있는지 체크
    public Sprite drinkIcon;
}

//손님 설계도
[System.Serializable]
public class GuestData
{
    public string guestName;//손님 이름
    public int unlockLevel;//등장 가능한 최소 레벨
    public string orderDrinkName;//주문할 음료 이름
    public Sprite guestIcon;
    [TextArea]
    public string dialogue;//대사

    public int currentSatisfaction = 0;// 현재 만족도
    public int maxSatisfaction = 100;// 성불에 필요한 만족도
    public bool isAscended = false;//성불여부. 성불하면 X
}