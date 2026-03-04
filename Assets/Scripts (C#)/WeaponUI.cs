using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WeaponUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private Image currentWeaponIcon;
    [SerializeField] private Image nextWeaponIcon;

    [Header("Panel")]
    [SerializeField] private GameObject upgradePanel;

    [Header("Buttons")]
    [SerializeField] private Button openBtn;   // 열기 버튼(WeaponUpBtn)
    [SerializeField] private Button buyButton; // 구매 버튼(WeaponPanel 안)

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI currentWeaponText;
    [SerializeField] private TextMeshProUGUI nextWeaponText;
    [SerializeField] private TextMeshProUGUI nextCostText;

    [Header("Pause")]
    [SerializeField] private bool pauseGame = true;

    private bool isOpen;

    private void Awake()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        if (openBtn != null)
            openBtn.onClick.AddListener(Open);
    }

    public void Open()
    {
        if (isOpen) return;
        if (upgradePanel == null) return;

        isOpen = true;
        upgradePanel.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.OpenInventory);
        Refresh();

        /*
        // 핵심: UI를 한 프레임 먼저 그리게 한 뒤 멈춤
        if (pauseGame)
            StartCoroutine(PauseNextFrame());
        */
    }

    private IEnumerator PauseNextFrame()
    {
        yield return null;            // 다음 프레임
        Time.timeScale = 0f;
    }

    public void Close()
    {
        if (!isOpen) return;
        if (upgradePanel == null) return;

        isOpen = false;
        upgradePanel.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.OpenInventory);
        if (pauseGame) Time.timeScale = 1f;
    }

    public void Refresh()
    {
        // ✅ null 체크를 맨 위로
        if (weapon == null || weaponInfo == null)
        {
            Debug.LogError("[WeaponUI] weapon/weaponInfo가 비어있음");
            return;
        }
        if (currentWeaponText == null || nextWeaponText == null || nextCostText == null)
        {
            Debug.LogError("[WeaponUI] TMP 텍스트가 비어있음");
            return;
        }

        int lv = weapon.Level;
        int idx = lv - 1;

        if (currentWeaponIcon != null && weaponInfo.weaponIcons != null &&
            idx >= 0 && idx < weaponInfo.weaponIcons.Length)
        {
            currentWeaponIcon.sprite = weaponInfo.weaponIcons[idx];
            currentWeaponIcon.enabled = (currentWeaponIcon.sprite != null);
        }

        if (nextWeaponIcon != null)
        {
            if (weapon.HasNext && weaponInfo.weaponIcons != null &&
                (idx + 1) >= 0 && (idx + 1) < weaponInfo.weaponIcons.Length)
            {
                nextWeaponIcon.sprite = weaponInfo.weaponIcons[idx + 1];
                nextWeaponIcon.enabled = (nextWeaponIcon.sprite != null);
            }
            else
            {
                nextWeaponIcon.sprite = null;
                nextWeaponIcon.enabled = false;
            }
        }

        currentWeaponText.text = $"Current: Lv.{weapon.Level} Damage: {weapon.Damage}";

        if (!weapon.HasNext)
        {
            nextWeaponText.text = "Next: MAX";
            nextCostText.text = "Cost: -";
            if (buyButton != null) buyButton.interactable = false;
            return;
        }

        nextWeaponText.text = $"Next: Lv.{weapon.Level + 1} Damage: ??";
        nextCostText.text = $"Cost: {weapon.NextPrice}";

        if (buyButton != null)
            buyButton.interactable = (GameManager.instance.money >= weapon.NextPrice);
    }

    public void OnClickBuy()
    {
        weapon.TryUpgrade();
        Refresh();
    }
}