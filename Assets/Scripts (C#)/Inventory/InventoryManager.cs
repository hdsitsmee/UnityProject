using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public InventoryData inventoryData;
    public InventoryUI inventoryUI;
    public static InventoryManager instance;

    private Dictionary<string, int> memoryCounts = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
    }

    public void AddItem(Item newItem)
    {
        if (inventoryData != null)
        {
            inventoryData.AddItem(newItem);
            inventoryUI.UpdateUI();
        }
    }

    public void AddMemory(MemoryData data)
    {
        if (inventoryData != null)
        {
            inventoryData.AddMemory(data);

            if (!memoryCounts.ContainsKey(data.ghostName))
                memoryCounts[data.ghostName] = 0;

            if (memoryCounts[data.ghostName] < 3)
                memoryCounts[data.ghostName]++;

            inventoryUI.UpdateUI();

            Debug.Log(data.ghostName + " count: " + memoryCounts[data.ghostName]);
        }
    }

    public int GetMemoryCount(string ghostName)
    {
        if (memoryCounts.ContainsKey(ghostName))
            return memoryCounts[ghostName];
        return 0;
    }
}