using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static int level = 1; //초기 레벨
    public static int money = 2500; //초기 돈

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

    //레벨업 함수
    public static void LevelUp()
    {
        level++;
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
}

//손님 설계도
[System.Serializable]
public class GuestData
{
    public string guestName;//손님 이름
    public int unlockLevel;//등장 가능한 최소 레벨
    public string orderDrinkName;//주문할 음료 이름
    [TextArea]
    public string dialogue;//대사
}