using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject collectionPopup; // 도감 팝업창 전체
    public Transform contentArea;      // 슬롯들이 들어갈 Scroll View의 Content
    public GameObject slotPrefab;      // 슬롯 프리팹

    [Header("Buttons")]
    public Button drinkTabButton;
    public Button guestTabButton;

    void Start()
    {
        // 시작하면 도감 끄기
        collectionPopup.SetActive(false);
    }

    // 도감 열기 버튼에 연결
    public void OpenCollection()
    {
        collectionPopup.SetActive(true);
        ShowDrinks(); // 기본으로 음료 탭 보여주기
    }

    // 도감 닫기 버튼에 연결
    public void CloseCollection()
    {
        collectionPopup.SetActive(false);
    }

    // 1. 음료 탭 클릭 시
    public void ShowDrinks()
    {
        ClearSlots(); // 기존 목록 지우기

        // GameManager에 있는 모든 레시피를 가져옴
        //[🥨변경] 기존 allRecipes -> recipebook.allRecipes
        foreach (var recipe in GameManager.instance.recipebook.allRecipes)
        {
            GameObject go = Instantiate(slotPrefab, contentArea);
            CollectionSlot slot = go.GetComponent<CollectionSlot>();

            // hasMade가 true면 해금
            slot.SetSlot(recipe.drinkName, recipe.drinkIcon, recipe.hasMade);
        }
    }

    // 2. 손님 탭 클릭 시
    public void ShowGuests()
    {
        ClearSlots(); // 기존 목록 지우기

        // GameManager에 있는 모든 손님 데이터를 가져옴
        foreach (var guest in GameManager.instance.allGuests)
        {
            GameObject go = Instantiate(slotPrefab, contentArea);
            CollectionSlot slot = go.GetComponent<CollectionSlot>();

            // hasMet이 true면 해금
            slot.SetSlot(guest.guestName, guest.guestIcon, guest.hasMet);
        }
    }

    // 슬롯 초기화 (싹 지우기)
    void ClearSlots()
    {
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
    }
}