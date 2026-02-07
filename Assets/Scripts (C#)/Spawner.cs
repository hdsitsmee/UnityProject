using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //유령 출현 위치 및 spawndata(유령 출현 시간,유령 레벨)
    public Transform spawnPoint;
    public SpawnData[] spawnData;

    int level;
    float timer;

    public GuestManager guestmanager;

    void Awake()
    {
        //구체적인 출현 위치 미정
        spawnPoint = GetComponent<Transform>();
        guestmanager = GetComponent<GuestManager>();
    }
    void Update()
    {

    }
    [System.Serializable]
    public class SpawnData
    {
        public float spawnTime;
        public int GhostType;
    }
}
