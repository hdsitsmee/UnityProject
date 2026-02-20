using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI countText;

    public Item item;
    public MemoryData memoryData;
    public bool isMemory;

    public void AddItem(Item newItem, int count)
    {
        isMemory = false;
        item = newItem;
        memoryData = null;
        icon.sprite = newItem.icon;
        icon.enabled = true;
        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);
    }

    public void AddMemory(MemoryData newData, int count)
    {
        isMemory = true;
        memoryData = newData;
        item = null;
        icon.sprite = newData.fragmentIcon;
        icon.enabled = true;
        countText.text = count.ToString();
        countText.gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        item = null;
        memoryData = null;
        icon.sprite = null;
        icon.enabled = false;
        countText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isMemory && memoryData != null)
        {
            int currentCount = int.Parse(countText.text);
            InventoryUI.Instance.ShowMemoryTooltip(memoryData, currentCount);
        }
        else if (item != null)
        {
            int currentCount = int.Parse(countText.text);
            InventoryUI.Instance.ShowTooltip(item, currentCount);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.Instance.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null || memoryData != null)
        {
            InventoryUI.Instance.PinTooltip(); 
        }
    }
}