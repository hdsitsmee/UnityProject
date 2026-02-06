using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class DropMaterialInfo
{
    public GameObject materialPrefab; // 드랍할 재료 프리팹
    public int minCount;
    public int maxCount;
    public float weight; // 가중치(클수록 잘 나옴)
}

[Serializable]
public class MonsterInfo
{
    [Header("몬스터 스탯")]
    public int level;
    public float maxHealth;
    [Header("몬스터 이동 관련")]
    public float speed;
    public float changeDirIntervalMin; 
    public float changeDirIntervalMax; //다음 이동까지 걸리는 시간을 최대~최소 사이 랜덤으로! 
    public Tilemap moveAreaTilemap;

    [Header("드랍 재료 리스트")]
    public List<DropMaterialInfo> dropMaterials = new List<DropMaterialInfo>();
}

[Serializable]
public class SpawnAreaInfo
{
    [Header("스폰 Area")]
    public int maxSpawnCount;

    [Header("몬스터 프리팹")]
    public GameObject monsterPrefab;

    [Header("몬스터 데이터")]
    public MonsterInfo monsterData = new MonsterInfo();
}

