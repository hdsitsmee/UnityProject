using UnityEngine;

public class Weapon : MonoBehaviour
{
    int nextWeaponPrice;
    int weaponLevel;
    public float weaponDamage;
    public WeaponInfo info;
    Rigidbody2D rigid;
    public void Init(WeaponInfo info) //  초기화하는 변수
    {
        weaponLevel = WeaponInfo.weaponLevel; // 무기레벨 설정 -> 초기값 1
        nextWeaponPrice = info.weaponPrice[weaponLevel]; // 다음 레벨업에 필요한 무기의 가격 
        weaponDamage = info.weaponDamage[weaponLevel - 1]; // 현재 소지한 무기의 데미지

    }
    private void Start()
    {
        Init(info);
        gameObject.SetActive(false);
        Debug.Log("무기 비활성화");
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void WeaponUpgrade()
    {
        if (GameManager.money >= nextWeaponPrice)
        {
            Debug.Log("무기 구매 가능");
            
            GameManager.money -= nextWeaponPrice;
            WeaponInfo.weaponLevel += 1;

        }
        else
        {
            Debug.Log("돈 부족, 무기 구매 불가능");
        }
        
    }
}