using UnityEngine;
using TMPro;
using System.Collections;

public class AcquisitionPopup : MonoBehaviour
{
    public static AcquisitionPopup instance; // 어디서든 부를 수 있게 싱글톤 설정
    public GameObject popupPanel; // 화면에 띄울 패널
    public TextMeshProUGUI infoText; // 아이템 이름이 들어갈 텍스트

    void Awake()
    {
        instance = this; // 자기 자신을 인스턴스로 등록
        popupPanel.SetActive(false); // 처음엔 꺼두기
    }

    public void ShowMessage(string itemName)
    {
        StopAllCoroutines(); // 이전에 돌던 팝업이 있으면 끄고 새로 시작
        StartCoroutine(PopupRoutine(itemName));
    }

    IEnumerator PopupRoutine(string itemName)
    {
        infoText.text = "Acquired: " + itemName;
        popupPanel.SetActive(true); // 팝업 켜기

        yield return new WaitForSeconds(1.5f); 

        popupPanel.SetActive(false); // 팝업 끄기
    }
}