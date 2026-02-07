using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GuestManager : MonoBehaviour
{
    public TMP_Text speechBubbleText; //말풍선 텍스트
    public GameObject guestImage; //유령 이미지

    public Button makeButton;

    [Header("손님 등장 설정")]
    public float spawnDelay = 3.0f;//손님이 없을 때 대기 시간 (3초)
    private float currentTimer = 0f;//타이머 계산용 변수

    void Start()
    {
        CheckGuestStatus();
        
    }
    void Update()
    {
        //현재 손님이 없는 경우에만 타이머가 돌아가게
        if (GameManager.instance.currentOrderName == "")
        {
            currentTimer += Time.deltaTime; //시간을 흐르게 함

            //시간이 다 되면 손님
            if (currentTimer >= spawnDelay)
            {
                CallGuest();
                currentTimer = 0f;//타이머 초기화
            }
        }
    }
    void CheckGuestStatus()
    {
        // 만약 이미 손님 있다면
        if (GameManager.instance.currentOrderName != "")
        {
            guestImage.SetActive(true);
            makeButton.interactable = true;
        }
        else
        {
            //손님이 없으면 다 끄고 대기
            guestImage.SetActive(false);
            makeButton.interactable = false;
        }
    }
    //손님 부르기
    public void CallGuest()
    {
        //등록된 손님 리스트 가져오기
        List<GuestData> guests = GameManager.instance.allGuests;

        if (guests.Count > 0)
        {
            //랜덤으로 손님 한 명 선택한다
            int randomIndex = Random.Range(0, guests.Count);
            GuestData selectedGuest = guests[randomIndex];

            //현재 손님을 GameManager에 등록(현재 게스트)
            GameManager.instance.currentGuest = selectedGuest;
            GameManager.instance.currentOrderName = selectedGuest.orderDrinkName;

            Image characterImg = guestImage.GetComponent<Image>();
            if (selectedGuest.guestIcon != null)
            {
                characterImg.sprite = selectedGuest.guestIcon;
                
                characterImg.SetNativeSize(); 
            }

            //연출-대사 출력 등...
            

            speechBubbleText.text = selectedGuest.dialogue + "\n<color=yellow>(order: " + selectedGuest.orderDrinkName + ")</color>";//손님 고유 대사 출력
            
            //성불하면 대사 달라질수도...
            guestImage.SetActive(true);
            makeButton.interactable = true;
            Debug.Log("손님 등장: " + selectedGuest.guestName + " / 주문: " + selectedGuest.orderDrinkName);
        }
        else
        {
            Debug.LogError("손님 데이터가 비어있음");
        }
    }
}
