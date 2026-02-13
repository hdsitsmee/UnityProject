using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;                // 아이템 아이콘
    public TextMeshProUGUI countText; // 수량 표시 텍스트

    private Item currentItem;         // 현재 슬롯에 담긴 아이템

    public void AddItem(Item newItem, int count)
    {
        if (newItem == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        currentItem = newItem;
        icon.sprite = newItem.icon; //
        icon.enabled = true;

        if (count > 1)
        {
            countText.text = count.ToString();
            countText.gameObject.SetActive(true);
        }
        else
        {
            countText.gameObject.SetActive(false); // 1개일 땐 숨기기
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        countText.gameObject.SetActive(false);
    }
}