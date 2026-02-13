using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


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

    void Awake()
    {
        Instance = this;

        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        // 패널 켜고 끄기
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        if (inventoryPanel.activeSelf)
        {
            UpdateUI();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
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
    public static InventoryUI Instance; 

    [Header("툴팁 UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI itemDescText;
    public TextMeshProUGUI itemCountText;
    public Image itemIcon;


    public void ShowTooltip(Item item, int count)
    {
        itemNameText.text = item.itemName; //
        itemLevelText.text = "LV. " + item.level; //
        itemDescText.text = item.description; //
        itemCountText.text = "보유 수량: " + count + "개";

        itemIcon.sprite = item.icon;
        if (item.icon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
        }
        else
        {
            itemIcon.enabled = false;
        }

        tooltipPanel.SetActive(true); 
    }

    public void HideTooltip() => tooltipPanel.SetActive(false);
}