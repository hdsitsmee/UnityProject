using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUI : MonoBehaviour
{
    public TMP_Text levelText;
    public TMP_Text moneyText;
    public Slider expSlider;

    void Start()
    {
        UpdateUI();//게임 시작 시 정보를 갱신
    }

    void Update() 
    {
        UpdateUI();//실시간으로 정보 갱신

        /* 테스트용: 스페이스 바 누르면 돈 추가
        if (Input.GetKeyDown(KeyCode.Space)){
            GameManager.AddMoney(100);
            UpdateUI();
        }
        */

        if (expSlider != null) 
        {
            expSlider.value = (float)GameManager.currentExp / (float)GameManager.maxExp;
        }
       
    }

    public void UpdateUI()
    {
        //텍스트 상자에 게임 메니저의 정보를 넣음
        levelText.text = "LV." + GameManager.level;
        moneyText.text = "Money: " + GameManager.money;
    }
}