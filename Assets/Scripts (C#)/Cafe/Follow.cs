using UnityEngine;

public class Follow : MonoBehaviour
{
    [Header("Target")]
    public GuestManager guestManager;   // 인스펙터에 GuestManager 드래그
    public Vector3 screenOffset;        // 화면에서 조금 띄우고 싶으면 여기 (x,y)

    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (guestManager == null || guestManager.spawnPoint == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(guestManager.spawnPoint.position);
        rect.position = screenPos + screenOffset;
    }
}