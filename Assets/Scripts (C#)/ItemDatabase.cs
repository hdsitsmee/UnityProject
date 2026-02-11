using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems; 

    public Item GetRandomItemByLevel(int level)
    {
        var filtered = allItems.Where(x => x.level == level).ToList();
        return filtered[Random.Range(0, filtered.Count)];
    }
}