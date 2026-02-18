using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems;


    public Item GetItemByMonsterLevel(int monsterLevel)
    {
        if (allItems == null) return null;

        List<int> targetItemLevels = new List<int>();

        switch (monsterLevel)
        {
            case 1: targetItemLevels.AddRange(new int[] { 1, 2 }); break;
            case 2: targetItemLevels.AddRange(new int[] { 2, 3 }); break;
            case 3: targetItemLevels.AddRange(new int[] { 3, 4 }); break;
            case 4: targetItemLevels.AddRange(new int[] { 4, 5 }); break;
            case 5: targetItemLevels.AddRange(new int[] { 6, 7 }); break;
            default: targetItemLevels.Add(1); break;
        }

        var filtered = allItems.Where(x => x != null && targetItemLevels.Contains(x.level)).ToList();

        if (filtered.Count > 0)
        {
            return filtered[Random.Range(0, filtered.Count)];
        }

        Debug.LogWarning(monsterLevel + "레벨 몬스터에 해당하는 아이템이 데이터베이스에 없어!");
        return null;
    }
}