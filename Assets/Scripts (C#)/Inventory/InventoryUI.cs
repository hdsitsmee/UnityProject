using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public InventoryData inventoryData;
    public Transform itemsParent;
    public GameObject inventoryPanel;

    InventorySlot[] slots;

    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf) UpdateUI();
    }

    public void UpdateUI()
    {
        if (inventoryData == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventoryData.items.Count)
            {
                slots[i].AddItem(inventoryData.items[i].item, inventoryData.items[i].count);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}