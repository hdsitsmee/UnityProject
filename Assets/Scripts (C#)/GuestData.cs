using UnityEngine;

[CreateAssetMenu(fileName = "GuestData", menuName = "Data_Cafe/GuestData")]
public class GuestData : ScriptableObject
{
    [Header("# Main Info")]
    public GameObject ghostPrefab; //[추가] 프리펩 저장 변수
    public string guestName; // 손님 이름
    public Sprite guestIcon;
    public int currentSatisfaction = 0; // 현재 만족도 (0부터 시작)
    public int maxSatisfaction = 100;   // 목표 만족도 (성불 기준, 기본 100)
    public string orderDrinkName; // 주문할 음료
    [Header("# Level Info")]
    public int unlockLevel; // 등장 레벨
    [Header("# Ascended/hasMet Info")]
    public bool isAscended = false; // 성불 여부
    public bool hasMet = false;
    [TextArea]
    public string dialogue; // 대사
}
