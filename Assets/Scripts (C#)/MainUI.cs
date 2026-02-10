using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUI : MonoBehaviour
{
    public TMP_Text levelText;
    public TMP_Text moneyText;
    [Header("Gauge UI")]
    public Slider expSlider;

    void Start()
    {
        UpdateUI();//게임 시작 시 정보를 갱신
    }

    void Update() 
    {
        UpdateUI();//실시간으로 정보 갱신
       
    }

    public void UpdateUI()
    {
        //텍스트 상자에 게임 메니저의 정보를 넣음
        levelText.text = "LV." + GameManager.level;
        moneyText.text = "Money: " + GameManager.money;
        if (expSlider != null && GameManager.instance != null)
        {
            // 슬라이더의 최대값을 '다음 레벨업에 필요한 경험치'로 설정
            expSlider.maxValue = GameManager.instance.maxExp;
            
            // 슬라이더의 현재값을 '내 현재 경험치'로 설정
            expSlider.value = GameManager.instance.currentExp;
        }
    }
}