using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;




[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerInfo : ScriptableObject
{

    [Header("플레이어 스탯")]

    public int level = GameManager.level; // 그냥 표시용 
    public float maxHealth = 10f;
    public float playerDamage = 0; // 나중에 무기시스템 완성되면 없애든지, 레벨에 따른 데미지 가산점 처리해도될듯

    [Header("플레이어 이동 관련")]
    public float speed = 3;

    [Header("플레이어 공격 관련")]
    public float attackTimer =0.5f;
    public float cooltimeTimer = 0.1f;

    //[Header("플레이어 소지금")]
    //public int money = GameManager.money;
}

[Serializable]
[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
public class WeaponInfo : ScriptableObject
{
    [Header("무기 이미지")]
    public Sprite[] weaponIcons;
    
    [Header("무기 레벨")]
    public int weaponLevel = 1; // 초기값 -> 런타임에 들고 있는 걸 추천
    public int maxWeaponLevel = 5; // 최고레벨

    [Header("무기 스탯")]
 
    public float[] weaponDamage = { 3f, 10f, 15f, 30f, 50f };


    [Header("무기 가격")]
   
    public int[] weaponPrice = { 100, 200, 300, 400, 500 };


}

