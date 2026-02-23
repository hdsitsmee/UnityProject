using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class MainUI : MonoBehaviour
{

    public static MainUI instance;

    [Header("Popups")]
    public GameObject levelUpPopup;
    public TMP_Text unlockedItemsText;

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
            GameManager.instance.isGamePaused = true; // 팝업 뜨는 동안 게임 일시정지

            if (unlockedItemsText != null && GameManager.instance != null)
            {
                int currentLevel = GameManager.level;
                
                // 1. 새로 해금된 손님 이름 찾기
                List<string> newGuests = new List<string>();
                foreach (var guest in GameManager.instance.allGuests)
                {
                    if (guest.unlockLevel == currentLevel)
                        newGuests.Add(guest.guestName);
                }

                // 2. 새로 해금된 음료 이름 찾기
                List<string> newDrinks = new List<string>();
                foreach (var drink in GameManager.instance.recipebook.allRecipes)
                {
                    if (drink.unlockLevel == currentLevel)
                        newDrinks.Add(drink.drinkName);
                }

                // 3. 화면에 띄울 메시지 조립
                string message = $"Lv {currentLevel}!\n\n";
                
                if (newGuests.Count > 0)
                    message += $"New Guest: {string.Join(", ", newGuests)}\n";
                if (newDrinks.Count > 0)
                    message += $"New Recipe: {string.Join(", ", newDrinks)}";

                unlockedItemsText.text = message; // 텍스트 적용
            }

            levelUpPopup.SetActive(true);
            
            // 레벨업 소리 재생
            if(SoundManager.instance != null && SoundManager.instance.levelUpSound != null) 
                SoundManager.instance.PlaySFX(SoundManager.instance.levelUpSound);
            
            // 글을 읽어야 하니 3초 동안 대기
            yield return new WaitForSeconds(3.0f); 

            levelUpPopup.SetActive(false);
            GameManager.instance.isGamePaused = false; // 일시정지 해제
        }
    }
}