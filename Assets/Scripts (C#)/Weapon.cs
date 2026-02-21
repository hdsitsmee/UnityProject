using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponInfo info;

    [Header("런타임")]
    [SerializeField] public int weaponLevel = 1; // 1부터
    [SerializeField] private float weaponDamage;
    [SerializeField] private int nextWeaponPrice;

    //밖에서 읽기 전용
    public int Level => weaponLevel;
    public float Damage => weaponDamage;
    public int NextPrice => nextWeaponPrice;

    //클래스 멤버(함수 밖)에 있어야 함
    public bool HasNext
    {
        get
        {
            int idx = weaponLevel - 1;
            return weaponLevel < info.maxWeaponLevel
                   && idx >= 0
                   && idx < info.weaponDamage.Length - 1; // 다음 데미지 존재
        }
    }

    public bool CanUpgrade
    {
        get
        {
            return HasNext && nextWeaponPrice > 0;
        }
    }

    private void Start()
    {
        Recalculate();
        gameObject.SetActive(false);
    }

    public void Recalculate()
    {
        int idx = weaponLevel - 1;

        // 현재 데미지 계산
        idx = Mathf.Clamp(idx, 0, info.weaponDamage.Length - 1);
        weaponDamage = info.weaponDamage[idx];

        // 다음 업그레이드 비용 계산 
        bool hasPrice = idx >= 0 && idx < info.weaponPrice.Length;
        bool hasNext = weaponLevel < info.maxWeaponLevel;

        nextWeaponPrice = (hasNext && hasPrice) ? info.weaponPrice[idx] : 0;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade) return false;

        if (GameManager.money < nextWeaponPrice)
        {
            Debug.Log("돈 부족");
            return false;
        }

        GameManager.money -= nextWeaponPrice;
        weaponLevel += 1;

        Recalculate();
        return true;
    }
}