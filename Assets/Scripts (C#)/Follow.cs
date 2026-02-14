using UnityEngine;

public class Follow : MonoBehaviour
{
    [Header("Target")]
    public GuestManager guestManager;
    public Vector2 viewportOffset = new Vector2(0f, 0.06f); // 화면 높이의 6% 위로

    RectTransform rect;
    Canvas canvas;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        if (guestManager == null || guestManager.spawnPoint == null) return;

        var cam = Camera.main;
        if (cam == null || canvas == null) return;

        // 1) 월드 → 뷰포인트(0~1)
        Vector3 vp = cam.WorldToViewportPoint(guestManager.spawnPoint.position);

        // 오브젝트가 카메라 뒤로 갈 때 숨기는 기능:
        // if (vp.z < 0f) { rect.gameObject.SetActive(false); return; }
        // else rect.gameObject.SetActive(true);

        // 2) 비율 오프셋 적용 (해상도 바뀌어도 "같은 비율" 유지)
        vp.x += viewportOffset.x;
        vp.y += viewportOffset.y;

        // 3) 뷰포인트 → 스크린 픽셀
        Vector2 screenPos = new Vector2(vp.x * Screen.width, vp.y * Screen.height);

        // 4) 스크린 → 캔버스 로컬
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out Vector2 localPos
        );

        // 5) anchoredPosition으로 적용
        rect.anchoredPosition = localPos;
    }
}