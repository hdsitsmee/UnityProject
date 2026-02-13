using System.Collections.Generic; 
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDatabase : ScriptableObject 
{
    public List<Item> allItems; //

    public Item GetRandomItemByLevel(int level) //
    {
        if (allItems == null) return null;

        var filtered = allItems.Where(x => x != null && x.level == level).ToList();

        if (filtered.Count > 0)
        {
            return filtered[Random.Range(0, filtered.Count)];
        }

        return null;
    }
}