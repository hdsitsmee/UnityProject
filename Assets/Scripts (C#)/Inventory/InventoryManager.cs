using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public InventoryData inventoryData;
    public InventoryUI inventoryUI;

    public void AddItem(Item newItem)
    {
        if (inventoryData != null)
        {
            // 1. 에셋에 아이템 추가 (중복 체크는 Data 안에서 함)
            inventoryData.AddItem(newItem);

            // 2. UI 새로고침
            inventoryUI.UpdateUI();
        }
    }
}