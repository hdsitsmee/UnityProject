using UnityEngine;
using System.Collections.Generic;



public class MemoryDatabase : MonoBehaviour
{
    public static MemoryDatabase instance;
    public List<MemoryData> allMemories; 


    public MemoryData GetMemoryByLevel(int level)
    {
        return allMemories.Find(m => m.dropLevel == level);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 있으면 새로 생긴 건 삭제
        }
    }
}