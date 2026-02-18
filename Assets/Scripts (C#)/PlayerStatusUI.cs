using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("# UI 연결")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moneyText;
    public Slider expSlider;

    void Update()
    {
        // 1. 레벨과 돈 업데이트
        levelText.text = "LV. " + GameManager.level;
        moneyText.text = GameManager.money.ToString("N0") + " G";

        // 2. 경험치 슬라이더 업데이트
        if (GameManager.instance != null && expSlider != null)
        {
            expSlider.maxValue = GameManager.instance.maxExp;
            expSlider.value = GameManager.instance.currentExp;
        }
    }
}