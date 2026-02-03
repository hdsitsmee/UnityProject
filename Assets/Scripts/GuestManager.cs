using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GuestManager : MonoBehaviour
{
    public TMP_Text speechBubbleText; //말풍선 텍스트
    public GameObject guestImage; //유령 이미지

    public Button makeButton;

    void Start()
    {
        //게임 시작할 때는 손님이 없으니 버튼을 끈다
        makeButton.interactable = false; 
        
        //유령 그림도 숨긴다
        guestImage.SetActive(false);
    }
    //손님 부르기
    public void CallGuest()
    {
        //GameManager에 등록된 모든 레시피를 가져온다
        List<DrinkRecipe> recipes = GameManager.instance.allRecipes;

        if (recipes.Count > 0)
        {
            //랜덤으로 하나
            int randomIndex = Random.Range(0, recipes.Count);
            DrinkRecipe selectedMenu = recipes[randomIndex];

            //주문 내용을 GameManager에 저장
            GameManager.instance.currentOrderName = selectedMenu.drinkName;

            //화면에 손님과 말풍선
            guestImage.SetActive(true); //유령 등장
            speechBubbleText.text = selectedMenu.drinkName;

            Debug.Log(selectedMenu.drinkName);
            makeButton.interactable = true;//버튼 활성화
        }
        else
        {
            Debug.LogError("메뉴판(Recipes)이 비어있습니다! GameManager를 확인하세요.");
        }
    }
}
