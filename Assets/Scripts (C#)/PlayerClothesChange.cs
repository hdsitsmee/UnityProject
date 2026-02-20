using UnityEngine;

public class PlayerClothesChange : MonoBehaviour
{
    
    [Header("자식 외형 오브젝트")]
    public GameObject cafeVisual;
    public GameObject battleVisual;

    void Awake()
    {
        SetInSafeZone(true); // 시작 복장 기본값
    }

    void SetInSafeZone(bool inSafe)
    {
        if (cafeVisual) cafeVisual.SetActive(inSafe);
        if (battleVisual) battleVisual.SetActive(!inSafe);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SafeZone")) //safezone에 들어갔으면 
            SetInSafeZone(true);//카페복장 활성화됨
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SafeZone")) //safezone에서 나갔으면
            SetInSafeZone(false);//전투복장 활성화됨 
    }
}
