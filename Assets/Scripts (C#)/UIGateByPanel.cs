using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGateByPanel : MonoBehaviour
{
    TMP_Text speechBubbleText;
    GameObject orderBullon;
    Slider patienceSlider;

    // Snapshot data
    // 도감 클릭 후 도감 닫힐 때까지 UI 상태를 저장하는 용도    
    // 도감 열릴 때 UI 숨김 동시에 직전 상태 저장
    bool hasSnapshot;

    bool prevOrderBullonActive;

    bool prevSpeechActive;
    string prevSpeechText;

    bool prevSliderActive;
    float prevSliderValue;

    void Awake()
    {
        // 인스턴스 준비 타이밍 이슈 있을 수 있으면 Start에서 보정하는 게 안전
        speechBubbleText = GuestManager.instance?.speechBubbleText;
        orderBullon = GuestManager.instance?.OrderBullon;
        patienceSlider = GuestManager.instance?.patienceSlider;
    }

    void OnEnable()
    {
        GuestManager.instance.SetPause(true); // 도감 열리는 동안 게임 진행 멈춤
        SaveSnapshot();      // 도감 열기 직전 상태 저장
        ApplyGate(true);     // UI 숨김
    }

    void OnDisable()
    {
        GuestManager.instance.SetPause(false); // 도감 닫히면 게임 진행 재개
        RestoreSnapshot();   // 직전 화면 저장한 그대로 복구
    }

    void SaveSnapshot()
    {
        if (hasSnapshot) return; // 중복 저장 방지
        hasSnapshot = true;

        if (orderBullon != null)
            prevOrderBullonActive = orderBullon.activeSelf;

        if (speechBubbleText != null)
        {
            prevSpeechActive = speechBubbleText.gameObject.activeSelf;
            prevSpeechText = speechBubbleText.text;
        }

        if (patienceSlider != null)
        {
            prevSliderActive = patienceSlider.gameObject.activeSelf;
            prevSliderValue = patienceSlider.value;
        }
    }

    void RestoreSnapshot()
    {
        if (!hasSnapshot) return;
        hasSnapshot = false;

        if (orderBullon != null)
            orderBullon.SetActive(prevOrderBullonActive);

        if (speechBubbleText != null)
        {
            speechBubbleText.text = prevSpeechText;
            speechBubbleText.gameObject.SetActive(prevSpeechActive);
        }

        if (patienceSlider != null)
        {
            patienceSlider.value = prevSliderValue;
            patienceSlider.gameObject.SetActive(prevSliderActive);
        }
    }

    void ApplyGate(bool gateOn)
    {
        if (!gateOn) return;

        // 도감 ON -> 강제 숨김
        if (orderBullon != null) orderBullon.SetActive(false);

        if (speechBubbleText != null)
        {
            speechBubbleText.text = "";
            speechBubbleText.gameObject.SetActive(false);
        }

        if (patienceSlider != null)
            patienceSlider.gameObject.SetActive(false);
    }
}
