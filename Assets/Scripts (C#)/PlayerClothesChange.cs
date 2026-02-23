using UnityEngine;

public class PlayerClothesChange : MonoBehaviour
{
    private Player player;
    [SerializeField] private SpriteRenderer cafeSprite;
    [SerializeField] private SpriteRenderer battleSprite;
    [Header("자식 외형 오브젝트")]
    public GameObject cafeVisual;
    public GameObject battleVisual;

    [Header("외형 animator")]
    [SerializeField] private Animator cafeAnim;    // ClothesCafe의 Animator
    [SerializeField] private Animator battleAnim;
    void Awake()
    {
        player = GetComponent<Player>();
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
    void LateUpdate()
    {
        float speed = player.moveDir.magnitude;

        if (cafeAnim != null) cafeAnim.SetFloat("speed", speed);
        if (battleAnim != null) battleAnim.SetFloat("speed", speed);

        if (cafeAnim) cafeAnim.SetFloat("speed", speed);
        if (battleAnim) battleAnim.SetFloat("speed", speed);

        if (Mathf.Abs(player.moveDir.x) > 0.01f)
        {
            bool left = player.moveDir.x < 0;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (left ? -1f : 1f);
            transform.localScale = s;
        }
        //if (Mathf.Abs(player.moveDir.x) > 0.01f)
        //{
        //    bool left = player.moveDir.x < 0;
        //    if (cafeSprite) cafeSprite.flipX = left;
        //    if (battleSprite) battleSprite.flipX = left;
        //}
    }
}
