using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public InventoryData inventoryData;
    public Transform itemsParent;
    public GameObject inventoryPanel;

    private InventorySlot[] slots;
    private bool isInventoryOpen = false;

    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI itemDescText;
    public TextMeshProUGUI itemCountText;
    public Image itemIcon;

    public GameObject memoryTooltipPanel;
    public TextMeshProUGUI memoryTitleText;
    public TextMeshProUGUI memoryLevelText;
    public TextMeshProUGUI memoryDescText;
    public Image memoryIcon;

    private bool isPinned = false;

    void Awake()
    {
        Instance = this;
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (memoryTooltipPanel != null) memoryTooltipPanel.SetActive(false);
    }

    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            UpdateUI();
            HideTooltip();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            HideTooltip();
        }

        if (!isInventoryOpen)
        {
            isPinned = false;
            HideTooltip();
        }
    }

    public void UpdateUI()
    {
        if (inventoryData == null || slots == null) return;
        int index = 0;
        for (int i = 0; i < inventoryData.items.Count && index < slots.Length; i++)
        {
            slots[index].AddItem(inventoryData.items[i].item, inventoryData.items[i].count);
            index++;
        }
        for (int i = 0; i < inventoryData.memories.Count && index < slots.Length; i++)
        {
            slots[index].AddMemory(inventoryData.memories[i].data, inventoryData.memories[i].count);
            index++;
        }
        for (int i = index; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
    }

    public void ShowTooltip(Item item, int count)
    {
        if (item == null) return;
        itemNameText.text = item.itemName;
        itemLevelText.text = "LV. " + item.level;
        itemDescText.text = item.description;
        itemCountText.text = "Amount: " + count;
        if (itemIcon != null) { itemIcon.sprite = item.icon; itemIcon.enabled = true; }
        tooltipPanel.SetActive(true);
    }

    public void ShowMemoryTooltip(MemoryData data, int count)
    {
        Debug.Log("넘어온 조각 개수: " + count);
        if (data == null) return;
        memoryTitleText.text = data.ghostName + "'s Memory";
        memoryLevelText.text = "LV. " + data.dropLevel;
        string story = "";
        for (int i = 0; i < count && i < data.stories.Count; i++) story += data.stories[i] + "\n\n";
        memoryDescText.text = story;
        if (memoryIcon != null) { memoryIcon.sprite = data.fragmentIcon; memoryIcon.enabled = true; }
        memoryTooltipPanel.SetActive(true);
    }

    public void PinTooltip()
    {
        isPinned = true; 
    }

    public void HideTooltip()
    {
        if (!isPinned)
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
            if (memoryTooltipPanel != null) memoryTooltipPanel.SetActive(false);
        }
    }

}