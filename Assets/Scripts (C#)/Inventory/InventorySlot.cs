using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;                // 아이템 아이콘
    public TextMeshProUGUI countText; // 수량 표시 텍스트
    public Item item;

    private Item currentItem;         // 현재 슬롯에 담긴 아이템

    public void AddItem(Item newItem, int count)
    {
        if (newItem == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        item = newItem;
        icon.sprite = newItem.icon; //
        icon.enabled = true;

        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        countText.gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            int currentCount = int.Parse(countText.text);
            InventoryUI.Instance.ShowTooltip(item, currentCount);
            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.Instance.HideTooltip();
    }
}