using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory/Data")]
public class InventoryData : ScriptableObject
{
    [System.Serializable]
    public class InventoryEntry
    {
        public Item item;
        public int count;
    }

    [System.Serializable]
    public class MemoryEntry
    { 
        public MemoryData data; 
        public int count; 
    }

    public List<InventoryEntry> items = new List<InventoryEntry>();

    public List<MemoryEntry> memories = new List<MemoryEntry>();

    public void AddItem(Item newItem)
    {
        InventoryEntry entry = items.Find(x => x.item == newItem);
        if (entry != null) entry.count++;
        else items.Add(new InventoryEntry { item = newItem, count = 1 });
    }

    public void AddMemory(MemoryData newMemory)
    {
        MemoryEntry entry = memories.Find(x => x.data == newMemory);
        if (entry != null)
        {
            if (entry.count < 3) entry.count++;
        }
        else
        {
            memories.Add(new MemoryEntry { data = newMemory, count = 1 });
        }
    }
    public int GetItemCount(string targetName)
    {
        InventoryEntry entry = items.Find(x => x.item != null && x.item.itemName == targetName);
        return entry != null ? entry.count : 0;
    }
    public void ConsumeItem(string targetName, int amount = 1)
    {
        InventoryEntry entry = items.Find(x => x.item != null && x.item.itemName == targetName);
        if (entry != null)
        {
            entry.count -= amount;
            if (entry.count <= 0)
            {
                items.Remove(entry); //0개가 되면 리스트에서 삭제한다
            }
        }
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "inventory.json");

    public void SaveToDisk()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(SavePath, json);
        Debug.Log("데이터 저장 완료!");
}

    public void LoadFromDisk()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log("데이터 복구 완료!");
    }
    }
}