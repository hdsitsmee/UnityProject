using UnityEngine;

public class InventoryDataReset : MonoBehaviour
{
    public InventoryData inventoryData;

    public void ResetInventory()
    {
        inventoryData.ResetInventoryData();
    }
}
