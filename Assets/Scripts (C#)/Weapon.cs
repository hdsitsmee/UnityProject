using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponInfo info;
    [SerializeField] private SpriteRenderer weaponRenderer;
    

    [Header("런타임")]
    [SerializeField] public int weaponLevel = 1; // 1����
    [SerializeField] private float weaponDamage;
    [SerializeField] private int nextWeaponPrice;
    // 공격키 한 번에 중복 타격 방지
    private readonly HashSet<int> hitThisSwing = new HashSet<int>();

    //�ۿ��� �б� ����
    public int Level => weaponLevel;
    public float Damage => weaponDamage;
    public int NextPrice => nextWeaponPrice;

    const string KEY_WEAPON_LEVEL="WeaponLevel";

    void OnEnable()
    {
        // 무기 켜질 때마다(=공격 시작) 초기화
        hitThisSwing.Clear();
    }


    //Ŭ���� ���(�Լ� ��)�� �־�� ��
    public bool HasNext
    {
        get
        {
            int idx = weaponLevel - 1;
            return weaponLevel < info.maxWeaponLevel
                   && idx >= 0
                   && idx < info.weaponDamage.Length - 1; // ���� ������ ����
        }
    }

    public bool CanUpgrade
    {
        get
        {
            return HasNext && nextWeaponPrice > 0;
        }
    }
    private void Awake()
    {
        if (info == null)
        {
            Debug.Log($"[WeaponLoader] Awake id={GetInstanceID()} scene={gameObject.scene.name} frame={Time.frameCount}");
            info = Resources.Load<WeaponInfo>("WeaponData");
            Debug.Log($"[WeaponLoader] Loaded={(info ? info.name : "NULL")}");
            
            if (info != null)
                Debug.Log("무기 데이터를 코드로 강제 복구했습니다!");
            else
                Debug.LogError("Resources 폴더 안에 WeaponData 파일이 없습니다!");
        }
        //rigid = GetComponent<Rigidbody2D>();
        if (weaponRenderer == null)
            weaponRenderer = GetComponent<SpriteRenderer>();

        Debug.Log($"[Weapon Awake] scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, obj={name}");

        if (info == null)
        {
            // ✅ Resources 내 전부 검색해서 출력 (경로/이름 문제를 한 방에 잡음)
            var all = Resources.LoadAll<WeaponInfo>("");
            Debug.Log($"[Weapon Awake] WeaponInfo assets found: {all.Length}");
            foreach (var a in all) Debug.Log($" - {a.name}");

            info = Resources.Load<WeaponInfo>("WeaponData");
            Debug.Log($"[Weapon Awake] Load('WeaponData') => {(info ? info.name : "NULL")}");
        }

        if (weaponRenderer == null)
            weaponRenderer = GetComponent<SpriteRenderer>();
        Load();    
    }

    public void Load()
    {
        weaponLevel=PlayerPrefs.GetInt(KEY_WEAPON_LEVEL,1);
    }
    public void Save()
    {
        PlayerPrefs.SetInt(KEY_WEAPON_LEVEL,weaponLevel);
        PlayerPrefs.Save(); //디스크에 
    }
    

    private void Start()
    {
       
        Recalculate();
        //gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 몬스터 태그/레이어 중 너 프로젝트 기준으로 맞춰
        if (!other.CompareTag("MonsterHitBox")) return;

        // Monster 스크립트가 부모에 있을 수도 있으니
        var hitbox = other.GetComponentInParent<Monster>();
        if (hitbox == null) return;

        int id = hitbox.gameObject.GetInstanceID();
        if (hitThisSwing.Contains(id)) return; // 이번 휘두르기에서 이미 맞음

        hitThisSwing.Add(id);

        // 데미지 적용 (Monster에 TakeDamage 같은 함수가 있으면 그걸 쓰는 게 더 좋음)
        hitbox.TakeDamage(Damage);

        // 필요하면 피격 SFX/이펙트
        // AudioManager.instance.PlaySfx(AudioManager.Sfx.MonsterHit);

    }

    public void Recalculate()
    {
        int idx = weaponLevel - 1;

        // ���� ������ ���
        idx = Mathf.Clamp(idx, 0, info.weaponDamage.Length - 1);
        weaponDamage = info.weaponDamage[idx];

        // ���� ���׷��̵� ��� ��� 
        bool hasPrice = idx >= 0 && idx < info.weaponPrice.Length;
        bool hasNext = weaponLevel < info.maxWeaponLevel;

        nextWeaponPrice = (hasNext && hasPrice) ? info.weaponPrice[idx] : 0;

        ApplyVisual();
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade) return false;

        if (GameManager.instance.money < nextWeaponPrice)
        {
            return false;
        }

        GameManager.instance.money -= nextWeaponPrice;
        weaponLevel += 1;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.SwordUpgrade);

        Recalculate();
        Save();
        return true;
    }
    private void ApplyVisual()
    {
        Debug.Log($"[ApplyVisual] name={name}, lvl={weaponLevel}, renderer={(weaponRenderer ? weaponRenderer.name : "NULL")}, spritesLen={(info && info.weaponSprites != null ? info.weaponSprites.Length : -1)}");
        if (info == null || weaponRenderer == null) return;

        int idx = weaponLevel - 1;
        if (info.weaponSprites == null || info.weaponSprites.Length == 0) return;

        idx = Mathf.Clamp(idx, 0, info.weaponSprites.Length - 1);
        weaponRenderer.sprite = info.weaponSprites[idx];
        Debug.Log($"[ApplyVisual] renderer={(weaponRenderer ? weaponRenderer.name : "NULL")}, sprite={(weaponRenderer && weaponRenderer.sprite ? weaponRenderer.sprite.name : "NULL")}");
    }
}