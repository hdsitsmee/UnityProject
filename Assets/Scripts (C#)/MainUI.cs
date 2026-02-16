using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainUI : MonoBehaviour
{

    public static MainUI instance;

    [Header("Popups")]
    public GameObject levelUpPopup;

    public TMP_Text levelText;
    public TMP_Text moneyText;
    [Header("Gauge UI")]
    public Slider expSlider;

    const float UI_UPDATE_INTERVAL = 0.3f; // 초당 약 3회 갱신 (매 프레임 대비 성능 개선)

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (levelUpPopup != null) levelUpPopup.SetActive(false);
        UpdateUI(); // 게임 시작 시 즉시 갱신
        InvokeRepeating(nameof(UpdateUI), UI_UPDATE_INTERVAL, UI_UPDATE_INTERVAL); // 주기적 갱신

        if (GameManager.instance != null && GameManager.instance.isLevelUpPending)
        {
            ShowLevelUpNotification();
            GameManager.instance.isLevelUpPending = false;
        }
    }

    void OnDisable()
    {
        CancelInvoke(nameof(UpdateUI)); // 비활성화 시 반복 호출 취소
    }

    public void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "LV." + GameManager.level;
        if (moneyText != null)
            moneyText.text = "Money: " + GameManager.money;
        if (expSlider != null && GameManager.instance != null)
        {
            // 슬라이더의 최대값을 '다음 레벨업에 필요한 경험치'로 설정
            expSlider.maxValue = GameManager.instance.maxExp;
            
            // 슬라이더의 현재값을 '내 현재 경험치'로 설정
            expSlider.value = GameManager.instance.currentExp;
        }
    }
    public void ShowLevelUpNotification()
    {
        
        StopAllCoroutines(); 
        StartCoroutine(LevelUpPopupRoutine());
    }
    IEnumerator LevelUpPopupRoutine()
    {
        if (levelUpPopup != null)
        {
            levelUpPopup.SetActive(true);
            
            if(SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.levelUpSound);

            yield return new WaitForSeconds(2.0f);

            levelUpPopup.SetActive(false);
        }
    }
}