using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public GameObject collectionPopup;//도감 팝업 전체
    public Transform contentGrid;//슬롯이 생성될 부모 위치
    public GameObject slotPrefab;//아까 만든 슬롯 프리팹
    public GameObject openButton;

    [Header("탭 버튼 색상용")]
    public Button recipeTabBtn;
    public Button guestTabBtn;
    
    //현재 보고 있는 탭 (0: 레시피, 1손님)
    private int currentTab = 0; 

    void Start()
    {
        //시작하면 일단 꺼두기
        collectionPopup.SetActive(false);
        if(openButton != null) openButton.SetActive(true);
    }

    //도감 열기 버튼에 연결
    public void OpenCollection()
    {
        collectionPopup.SetActive(true);
        if (openButton != null) openButton.SetActive(false);
        ShowRecipeTab();
    }

    public void CloseCollection()
    {
        collectionPopup.SetActive(false);
        if (openButton != null) openButton.SetActive(true);
    }

    //탭 1:레시피
    public void ShowRecipeTab()
    {
        currentTab = 0;
        ClearSlots(); //기존 목록 지우기

        foreach (var recipe in GameManager.instance.allRecipes)
        {
            //슬롯 생성
            GameObject newSlot = Instantiate(slotPrefab, contentGrid);
            CollectionSlot slotScript = newSlot.GetComponent<CollectionSlot>();

            slotScript.SetData(recipe.drinkName, recipe.drinkIcon, recipe.hasMade);
        }
    }

    //탭 2: 손님
    public void ShowGuestTab()
    {
        currentTab = 1;
        ClearSlots();

        foreach (var guest in GameManager.instance.allGuests)
        {
            GameObject newSlot = Instantiate(slotPrefab, contentGrid);
            CollectionSlot slotScript = newSlot.GetComponent<CollectionSlot>();

            //손님은 isAscended(성불) 여부로 해금 판단
            slotScript.SetData(guest.guestName, guest.guestIcon, guest.isAscended);
        }
    }

    //기존 슬롯들 싹 지우는 청소 함수
    void ClearSlots()
    {
        foreach (Transform child in contentGrid)
        {
            Destroy(child.gameObject);
        }
    }
}