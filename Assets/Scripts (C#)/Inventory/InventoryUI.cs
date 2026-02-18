using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("기본 데이터")]
    public InventoryData inventoryData;
    public Transform itemsParent;
    public GameObject inventoryPanel;

    private InventorySlot[] slots;
    private bool isInventoryOpen = false;

    [Header("툴팁 UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI itemDescText;
    public TextMeshProUGUI itemCountText;
    public Image itemIcon;

    void Awake()
    {
        Instance = this;
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
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
        if (inventoryPanel == null) return;

        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            UpdateUI();
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = 1f;
            HideTooltip(); 
        }
    }

    public void UpdateUI()
    {
        if (inventoryData == null || slots == null) return;

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

    public void ShowTooltip(Item item, int count)
    {
        if (item == null) return;

        itemNameText.text = item.itemName;
        itemLevelText.text = "LV. " + item.level;
        itemDescText.text = item.description;
        itemCountText.text = "Amount: " + count; 

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