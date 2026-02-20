using UnityEngine;
using System.Collections.Generic;

public class MemoryDatabase : MonoBehaviour
{
    public static MemoryDatabase instance;
    public List<MemoryData> allMemories; 

    void Awake() => instance = this;

    public MemoryData GetMemoryByLevel(int level)
    {
        return allMemories.Find(m => m.dropLevel == level);
    }
}