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
    public float temp;
}



