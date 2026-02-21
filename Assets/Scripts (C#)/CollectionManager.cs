using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CollectionManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject collectionPopup; // 도감 팝업창 전체
    public Transform contentArea;      // 슬롯들이 들어갈 Scroll View의 Content
    public GameObject slotPrefab;      // 슬롯 프리팹

    [Header("Buttons")]
    public Button drinkTabButton;
    public Button guestTabButton;

    [Header("Guest Detail Popup")]
    public GameObject guestDetailPopup;
    public TMP_Text detailNameText;
    public TMP_Text detailDialogueText;
    public Image detailIconImage;

    void Start()
    {
        // 시작하면 도감 끄기
        collectionPopup.SetActive(false);
        if (guestDetailPopup != null) guestDetailPopup.SetActive(false);
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
        if (guestDetailPopup != null) guestDetailPopup.SetActive(false);
    }

    public void CloseGuestDetail()
    {
        if (guestDetailPopup != null) guestDetailPopup.SetActive(false);
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

            if (slot.slotButton != null) slot.slotButton.onClick.RemoveAllListeners();
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

            if (slot.slotButton != null)
            {
                slot.slotButton.onClick.RemoveAllListeners();
                
                // 버튼 클릭 시 현재 순회의 guest 데이터를 넘겨줌
                slot.slotButton.onClick.AddListener(() => OpenGuestDetail(guest));
            }
        }
    }

    private void OpenGuestDetail(GuestData guest)
    {
        if (guestDetailPopup == null) return;

        guestDetailPopup.SetActive(true);
        
        if (detailNameText != null) detailNameText.text = guest.guestName;
        if (detailIconImage != null) detailIconImage.sprite = guest.guestIcon;

        //성불 여부에 따른 대사
        if (detailDialogueText != null)
        {
            if (guest.isAscended)
            {
                //성불 완료 시 진짜 대사
                detailDialogueText.text = guest.ascendedDialogue;
            }
            else
            {
                //성불 전이면 ???
                detailDialogueText.text = "???";
            }
        }
    }

    //슬롯 초기화
    void ClearSlots()
    {
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
    }
}