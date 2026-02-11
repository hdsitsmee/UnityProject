using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rect;

    [SerializeField] Vector2 screenOffset; // 픽셀 단위 (x,y)
    [SerializeField] bool useLateUpdate = true;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (!useLateUpdate) return;
        FollowWithOffset();
    }

    void FixedUpdate()
    {
        if (useLateUpdate) return;
        FollowWithOffset();
    }

    void FollowWithOffset()
    {
        Vector3 sp = Camera.main.WorldToScreenPoint(GameManager.instance.SpawnPoint.transform.position);
        rect.position = sp + (Vector3)screenOffset; // <- 여기서 x/y 따로 조절됨
    }
}