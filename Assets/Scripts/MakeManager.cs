using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MakeManager : MonoBehaviour
{
    public List<string> currentIngredients = new List<string>();
    public Button finishButton;
    public string testOrderName = "great water";

    [Header("색상 설정")]
    //선택되었을 때 바뀔 색깔
    public Color selectedColor = Color.gray; 
    //원래 버튼 색깔
    public Color normalColor = Color.white;
    public Color tutorialHighlightColor = new Color(1f, 1f, 0.5f); //튜토리얼 용 컬러(힌트)

    [Header("버튼 관리")]
    public GameObject[] allIngredientButtons;

    [Header("성불 시스템")]
    public Slider nirvanaSlider;//성불 게이지
    public GameObject resultPopup;//결과창


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
        CheckAndShowTutorial();
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
    void GoToMain()
    {
        GameManager.instance.currentOrderName = "";
        GameManager.instance.currentGuest = null;
        SceneManager.LoadScene("MainScene");
    }
    System.Collections.IEnumerator WaitAndGoMain()
{
    Debug.Log("4초 뒤 메인으로 이동");
    
    yield return new WaitForSecondsRealtime(4.0f);
    
    Debug.Log("메인으로 이동");
    GoToMain();
}
    //채점 로직
    void CheckResult(DrinkRecipe recipe)
    {
        int matchCount = 0;
        int score = 0;

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
            GameManager.instance.AddExp(100);
            score = 30;
            recipe.hasMade = true;

        }
        else
        {
            Debug.Log("실패");
            Debug.Log("필요한 재료 수: " + recipe.requiredIngredients.Length + " / 맞춘 개수: " + matchCount);
        }
       if (GameManager.instance.currentGuest != null)
        {
            GuestData guest = GameManager.instance.currentGuest;
            guest.currentSatisfaction += score;
            
            if (guest.currentSatisfaction >= guest.maxSatisfaction) 
            {
                guest.currentSatisfaction = guest.maxSatisfaction;
                if (!guest.isAscended)
                {
                    guest.isAscended = true;
                    Debug.Log(guest.guestName + "님이 성불 스토리 해금");
                }
            }

            //성불 게이지 UI 업데이트
            if (nirvanaSlider != null)
            {
                resultPopup.SetActive(true); //결과창 띄우기
                nirvanaSlider.maxValue = guest.maxSatisfaction;
                nirvanaSlider.value = guest.currentSatisfaction;
            }
        }
       StartCoroutine(WaitAndGoMain());
    }

    void CheckAndShowTutorial() //튜토리얼 로직
    {
        //현재 주문한 음료의 레시피를 가져옴
        DrinkRecipe recipe = GameManager.instance.GetRecipeByName(testOrderName);

        //레시피가 있고 + 아직 만들어본 적이 없다
        if (recipe != null && recipe.hasMade == false)
        {
            Debug.Log("튜토리얼 모드");
            //모든 버튼을 하나씩 훑어보면서 정답인지 확인
            foreach (GameObject btn in allIngredientButtons)
            {
                string btnName = btn.name;

                //이 버튼의 이름이 레시피 재료 목록에 포함되어 있는가?
                foreach (string requiredName in recipe.requiredIngredients)
                {
                    if (btnName == requiredName)
                    {
                        //정답 재료일때 색깔을 힌트 색상으로 변경
                        btn.GetComponent<Image>().color = tutorialHighlightColor;
                    }
                }
            }
        }
    }
}