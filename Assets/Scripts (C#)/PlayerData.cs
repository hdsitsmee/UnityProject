using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;



[Serializable]
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerInfo : ScriptableObject
{
    
    [Header("플레이어 스탯")]
    
    public float maxHealth = 10f;
    public float playerDamage = 5f;

    [Header("플레이어 이동 관련")]
    public float speed = 3;

    [Header("플레이어 공격 관련")]
    public float attackTimer =0.5f;
    public float cooltimeTimer = 0.1f;
}


[Serializable]
public class WeaponInfo : ScriptableObject
{

    [Header("무기 레벨")]
    public int weaponLevel;

    [Header("무기 스탯")]

    public float WeaponDamage = 5f;


  
}