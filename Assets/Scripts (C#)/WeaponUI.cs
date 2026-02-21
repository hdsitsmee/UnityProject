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
        Refresh();

        if (pauseGame) Time.timeScale = 0f;
    }

    public void Close()
    {
        if (upgradePanel == null) return;

        isOpen = false;
        upgradePanel.SetActive(false);

        if (pauseGame) Time.timeScale = 1f;
    }


    public void Refresh()
    {
        int lv = weapon.Level;     // 1부터
        int idx = lv - 1;          // 0부터

        // 현재 아이콘
        if (currentWeaponIcon != null && weaponInfo.weaponIcons != null && idx >= 0 && idx < weaponInfo.weaponIcons.Length)
        {
            currentWeaponIcon.sprite = weaponInfo.weaponIcons[idx];
            currentWeaponIcon.enabled = (currentWeaponIcon.sprite != null);
        }

        // 다음 아이콘
        if (nextWeaponIcon != null)
        {
            if (weapon.HasNext && weaponInfo.weaponIcons != null && (idx + 1) >= 0 && (idx + 1) < weaponInfo.weaponIcons.Length)
            {
                nextWeaponIcon.sprite = weaponInfo.weaponIcons[idx + 1];
                nextWeaponIcon.enabled = (nextWeaponIcon.sprite != null);
            }
            else
            {
                nextWeaponIcon.sprite = null;
                nextWeaponIcon.enabled = false; // MAX면 숨김
            }
        }
        if (weapon == null)
        {
            Debug.LogError("[WeaponUI] weapon이 Inspector에서 None임");
            return;
        }
        if (currentWeaponText == null || nextWeaponText == null || nextCostText == null)
        {
            Debug.LogError("[WeaponUI] TMP 텍스트 연결이 None임 (Current/Next/Cost 중 하나)");
            return;
        }
        Debug.Log($"[WeaponUI] Level={weapon.Level}, NextPrice={weapon.NextPrice}, HasNext={weapon.HasNext}");
        currentWeaponText.text = $"현재 무기: Lv.{weapon.Level} / 데미지 : {weapon.Damage}";

        if (!weapon.HasNext)
        {
            nextWeaponText.text = "다음 무기: MAX";
            nextCostText.text = "필요 금액: -";
            if (buyButton != null) buyButton.interactable = false;
            return;
        }

        nextWeaponText.text = $"다음 무기: Lv.{weapon.Level + 1}";
        nextCostText.text = $"필요 금액: {weapon.NextPrice}";

        if (buyButton != null) buyButton.interactable = (GameManager.money >= weapon.NextPrice);
    }

    public void OnClickBuy()
    {
        weapon.TryUpgrade();
        Refresh();
    }
}

