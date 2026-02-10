using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; 
    bool activeInventory = false; // 현재 인벤토리가 켜져 있는지 확인

    void Start()
    {
        // 게임 시작할 때는 인벤토리를 꺼둠
        inventoryPanel.SetActive(activeInventory);
    }

    void Update()
    {
        // 'I' 키를 누르면 상태를 반전시킴
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
    }

    // 닫기 버튼(X) 전용 함수
    public void CloseInventory()
    {
        activeInventory = false;
        inventoryPanel.SetActive(false);
    }
}