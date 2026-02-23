using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    
    [Header("참조(씬 오브젝트)")]
    [SerializeField] private Weapon weapon;              // Weapon.cs 붙은 오브젝트
    [SerializeField] private WeaponInfo weaponInfo;      // WeaponInfo.asset (테이블)
    [SerializeField] private Image currentWeaponIcon;
    [SerializeField] private Image nextWeaponIcon;
    

    [Header("패널")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button openBtn;

    [Header("UI 텍스트")]
    [SerializeField] private TextMeshProUGUI currentWeaponText;
    [SerializeField] private TextMeshProUGUI nextWeaponText;
    [SerializeField] private TextMeshProUGUI nextCostText;

    [Header("구매 버튼")]
    [SerializeField] private Button buyButton;
    

    [Header("열릴 때 게임 멈출지")]
    [SerializeField] private bool pauseGame = true;

    private bool isOpen;

    private void Awake()
    {
        
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    public void Open()
    {
        
        if (upgradePanel == null) return;

        isOpen = true;
        upgradePanel.SetActive(true);
        //if (openBtn != null) openBtn.interactable = false;
        Refresh();

        if (pauseGame) Time.timeScale = 0f;

        
    }

    public void Close()
    {
        
        if (upgradePanel == null) return;

        //if (openBtn != null) openBtn.interactable = true;
        isOpen = false;

        

        if (pauseGame) Time.timeScale = 1f;


        upgradePanel.SetActive(false);
    }

    public void Refresh()
    {
        //// 1) 필수 참조 null 체크 (가장 먼저)
        //if (weapon == null)
        //{
        //    Debug.LogError("[WeaponUI] weapon이 Inspector에서 None임");
        //    return;
        //}
        //if (weaponInfo == null)
        //{
        //    Debug.LogError("[WeaponUI] weaponInfo가 Inspector에서 None임");
        //    return;
        //}
        //if (currentWeaponText == null || nextWeaponText == null || nextCostText == null)
        //{
        //    Debug.LogError("[WeaponUI] TMP 텍스트 연결이 None임 (Current/Next/Cost 중 하나)");
        //    return;
        //}

        int lv = weapon.Level;     // 1부터
        int idx = lv - 1;          // 0부터

        // 2) 텍스트 갱신
        currentWeaponText.text = $"현재 무기: Lv.{lv} / 데미지 : {weapon.Damage}";

        if (!weapon.HasNext)
        {
            nextWeaponText.text = "다음 무기: MAX";
            nextCostText.text = "필요 금액: -";
            if (buyButton != null) buyButton.interactable = false;
        }
        else
        {
            nextWeaponText.text = $"다음 무기: Lv.{lv + 1}";
            nextCostText.text = $"필요 금액: {weapon.NextPrice}";
            if (buyButton != null) buyButton.interactable = (GameManager.money >= weapon.NextPrice);
        }

        // 3) 아이콘 갱신
        var icons = weaponInfo.weaponIcons; // 줄여쓰기
        if (icons != null && icons.Length > 0)
        {
            // 현재 아이콘
            if (currentWeaponIcon != null && idx >= 0 && idx < icons.Length)
            {
                currentWeaponIcon.sprite = icons[idx];
                currentWeaponIcon.enabled = (currentWeaponIcon.sprite != null);
                currentWeaponIcon.preserveAspect = true;
            }

            // 다음 아이콘
            if (nextWeaponIcon != null)
            {
                int nextIdx = idx + 1;
                if (weapon.HasNext && nextIdx >= 0 && nextIdx < icons.Length)
                {
                    nextWeaponIcon.sprite = icons[nextIdx];
                    nextWeaponIcon.enabled = (nextWeaponIcon.sprite != null);
                    nextWeaponIcon.preserveAspect = true;
                }
                else
                {
                    nextWeaponIcon.sprite = null;
                    nextWeaponIcon.enabled = false;
                }
            }
        }

        
    }


    public void OnClickBuy()
    {
        weapon.TryUpgrade();
        Refresh();
    }
}

