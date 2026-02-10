using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory/Data")]
public class InventoryData : ScriptableObject
{
    [System.Serializable]
    public class InventoryEntry
    {
        public Item item;
        public int count;
    }

    // 아이템들이 담길 리스트
    public List<InventoryEntry> items = new List<InventoryEntry>();

    public void AddItem(Item newItem)
    {
        // 이미 있는 템이면 숫자만 올리고, 없으면 새로 추가
        InventoryEntry entry = items.Find(x => x.item == newItem);
        if (entry != null) entry.count++;
        else items.Add(new InventoryEntry { item = newItem, count = 1 });
    }
}