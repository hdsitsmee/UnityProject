using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MakeManager : MonoBehaviour
{
    public List<string> currentIngredients = new List<string>();

    public Button finishButton;
    //선택되었을 때 바뀔 색깔
    public Color selectedColor = Color.gray; 
    //원래 버튼 색깔
    public Color normalColor = Color.white;


    //테스트용 주문
    [Header("테스트용 주문 입력")]
    public string testOrderName = "great water";



    void Start()
    {
        if (GameManager.instance.currentOrderName != "")
        {
            testOrderName = GameManager.instance.currentOrderName;
            Debug.Log("메뉴는 " + testOrderName);
        }
        else
        {
            Debug.Log("x");
        }

        CheckFinishCondition();
    }

    public void ToggleIngredient(GameObject btnObj)
    {
        //버튼 오브젝트의 이름(Hierarchy 창의 이름)을 가져온다
        string name = btnObj.name; 
        
        //버튼에 붙어있는 이미지 컴포넌트를 찾아냅니다.
        Image buttonImage = btnObj.GetComponent<Image>();

        if (currentIngredients.Contains(name))
        {
            currentIngredients.Remove(name);
            buttonImage.color = normalColor;
            Debug.Log(name + " 취소됨");
        }
        else
        {
            currentIngredients.Add(name);
            buttonImage.color = selectedColor;
            Debug.Log(name + " 선택됨");
        }

        PrintCurrentStatus();
        CheckFinishCondition();//재료 넣고 뺄때마다 다시 검사
    }

    void PrintCurrentStatus()
    {
        string listString = "";
        foreach(string ing in currentIngredients) listString += ing + ", ";
        Debug.Log("현재 믹스: [ " + listString + " ]");
    }

    void CheckFinishCondition()
    {
        //재료 리스트의 개수가 0보다 크면 true 아니면 false
        if (currentIngredients.Count > 0)
        {
            finishButton.interactable = true; //버튼 활성
        }
        else
        {
            finishButton.interactable = false; //버튼 비활성화
        }
    }

    public void OnClickFinish()
    {
        // 게임매니저에게 레시피 정보 획득
        DrinkRecipe recipe = GameManager.instance.GetRecipeByName(testOrderName);

        if (recipe != null)
        {
            //채점
            CheckResult(recipe);
        }
        else
        {
            Debug.LogError("주문한 음료의 레시피를 찾을 수 없습니다!");
        }
    }

    //채점 로직
    void CheckResult(DrinkRecipe recipe)
    {
        int matchCount = 0;

        //레시피에 있는 재료가 내 컵에 몇 개나 들어있나 확인
        foreach (string required in recipe.requiredIngredients)
        {
            if (currentIngredients.Contains(required))
            {
                matchCount++;
            }
        }

        //간단한 판정 로직- 필요한 재료를 모두 넣었으면 성공
        if (matchCount == recipe.requiredIngredients.Length && currentIngredients.Count == recipe.requiredIngredients.Length)
        {
            Debug.Log("성공");
            GameManager.AddMoney(500); //500원
        }
        else
        {
            Debug.Log("실패");
            Debug.Log("필요한 재료 수: " + recipe.requiredIngredients.Length + " / 맞춘 개수: " + matchCount);
        }
        GameManager.instance.currentOrderName = "";

        //메인 화면으로 돌아가기
        SceneManager.LoadScene("MainScene");
    }
}