using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;       // 아이템 이름
    public int level;             // 아이템 레벨 (1~5)
    [TextArea] public string description; // 설명 (여러 줄 입력 가능)
    public Sprite icon;           // 인벤토리에 보일 이미지
}