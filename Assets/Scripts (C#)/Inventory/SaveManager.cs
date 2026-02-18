using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<string> itemNames = new List<string>();
}

public class SaveManager : MonoBehaviour
{
    public InventoryData inventoryData;
    public ItemDatabase itemDatabase;

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveInventory()
    {
        if (inventoryData == null) return;

        SaveData data = new SaveData();
        foreach (var entry in inventoryData.items)
        {
            if (entry != null && entry.item != null)
            {
                data.itemNames.Add(entry.item.itemName);
            }
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log("저장 완료: " + savePath);
    }

    public void LoadInventory()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (inventoryData != null)
        {
            inventoryData.items.Clear();

            foreach (string name in data.itemNames)
            {
                Item foundItem = itemDatabase.allItems.Find(x => x.itemName == name);
                if (foundItem != null)
                {
                    inventoryData.AddItem(foundItem);
                }
            }
            Debug.Log("불러오기 완료!");
        }
    }
}