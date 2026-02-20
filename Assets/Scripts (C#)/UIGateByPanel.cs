using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGateByPanel : MonoBehaviour
{
    public GameObject guestManager;
    TMP_Text speechBubbleText;
    GameObject orderBullon;
    Slider patienceSlider;

    void Awake()
    {
        // 인스턴스 준비 타이밍 이슈 있을 수 있으면 Start에서 보정하는 게 안전
        speechBubbleText = GuestManager.instance?.speechBubbleText;
        orderBullon = GuestManager.instance?.OrderBullon;
        patienceSlider = GuestManager.instance?.patienceSlider;
    }

    void OnEnable()
    {
        GameManager.instance.SetPause(true); // 도감 열리는 동안 게임 진행 멈춤
        ApplyGate();     // UI 숨김
    }

    void OnDisable()
    {
        GameManager.instance.SetPause(false); // 도감 닫히면 게임 진행 재개
        RestoreSnapshot();   // 직전 화면 저장한 그대로 복구
    }


    void RestoreSnapshot()
    {
        /*if (!hasSnapshot) return;
        hasSnapshot = false;*/

        if (orderBullon != null)
            orderBullon.SetActive(true);

        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(true);
        }

        if (patienceSlider != null)
        {
            PatienceUI(true);
        }
    }

    void ApplyGate()
    {
        // 도감 ON -> 강제 숨김
        if (orderBullon != null) orderBullon.SetActive(false);

        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(false);
        }

        if (patienceSlider != null)
            PatienceUI(false);
    }

    void PatienceUI(bool flag)
    {
        foreach (Transform child in patienceSlider.transform)
        {
            child.gameObject.SetActive(flag);
        }
    }
      
}
