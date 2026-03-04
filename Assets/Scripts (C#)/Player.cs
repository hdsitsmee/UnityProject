using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Transform weaponPivot;
    public GameObject weapon;
    Animator anim;
    [Header("플레이어 기본 정보")]
    public PlayerInfo info;
    public int playerMoney;
    
   

    [Header("공격 관련 스텟")]
    public float playerDamage;
    public float speed;
    public float health;


    
    public float attackTimer;//weapon 지속시간
    public float cooltimeTimer; // 공격 쿨타임 
    //public Vector2 inputVec;
    bool canAttack;
    float timer; //회전 공격에 사용
    public float maxHealth;
    public Transform spawnPoint;
    Vector2 lastAimDir = Vector2.down;
    public Vector2 moveDir { get; private set; }                 // 실제 이동 방향(우선순위 적용된 값)
    Vector2 lastHeldDir = Vector2.down;
    
    void Init()
    {
        ApplyPlayerInfo(info);

        canAttack = true;
        transform.position = spawnPoint.position; //스폰위치 재설정
        health = maxHealth; //초기 체력 설정
        rigid.linearVelocity = Vector2.zero;
        gameObject.SetActive(true); 
    }
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        
    }

    private void Start()
    {
        Init(); 
    }
    private void FixedUpdate() 
    {
        if (Time.timeScale == 0f) return;
        Vector2 nextVec = moveDir * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }
    private void Update()
    {
        playerMoney = GameManager.instance.money; //카페씬에서 번 돈 가져옴
        if (Time.timeScale == 0f) return;
        UpdateMoveByLastPressedKey();   // ★ 이걸로 moveDir 갱신
    }
    bool facingLeft;

    [SerializeField] SpriteRenderer cafeSR;
    [SerializeField] SpriteRenderer battleSR;

    void LateUpdate()
    {
        if (Time.timeScale == 0f) return;

        // 이동 중일 때만 방향 갱신
        if (moveDir.sqrMagnitude > 0.01f)
        {
            lastAimDir = moveDir.normalized;

            // 좌우 입력일 때만 flip 갱신 (상/하는 그대로)
            if (Mathf.Abs(moveDir.x) > 0.01f)
            {
                facingLeft = moveDir.x < 0;

                if (cafeSR != null) cafeSR.flipX = facingLeft;
                if (battleSR != null) battleSR.flipX = facingLeft;
            }
        }
    } 
        void UpdateMoveByLastPressedKey()
    {
        var kb = Keyboard.current;
        if (kb == null)
        {
            moveDir = Vector2.zero;
            return;
        }

        // 어떤 방향키가 지금 눌려있는지
        bool leftHeld  = kb.aKey.isPressed || kb.leftArrowKey.isPressed;
        bool rightHeld = kb.dKey.isPressed || kb.rightArrowKey.isPressed;
        bool upHeld    = kb.wKey.isPressed || kb.upArrowKey.isPressed;
        bool downHeld  = kb.sKey.isPressed || kb.downArrowKey.isPressed;

        // "마지막으로 눌린" 키를 우선순위로 갱신
        if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame)  lastHeldDir = Vector2.left;
        if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) lastHeldDir = Vector2.right;
        if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)    lastHeldDir = Vector2.up;
        if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame)  lastHeldDir = Vector2.down;

        // 아직 어떤 키가 눌려있나?
        bool anyHeld = leftHeld || rightHeld || upHeld || downHeld;
        if (!anyHeld)
        {
            moveDir = Vector2.zero;
            return;
        }

        // lastHeldDir 방향이 "아직도 눌려있으면" 그 방향으로 이동
        if (lastHeldDir == Vector2.left  && leftHeld)  { moveDir = Vector2.left;  return; }
        if (lastHeldDir == Vector2.right && rightHeld) { moveDir = Vector2.right; return; }
        if (lastHeldDir == Vector2.up    && upHeld)    { moveDir = Vector2.up;    return; }
        if (lastHeldDir == Vector2.down  && downHeld)  { moveDir = Vector2.down;  return; }

        // lastHeldDir 키가 떼졌으면, 남아있는 키 중 하나로 fallback (원하는 우선순위로)
        if (rightHeld) { moveDir = Vector2.right; return; }
        if (leftHeld)  { moveDir = Vector2.left;  return; }
        if (upHeld)    { moveDir = Vector2.up;    return; }
        if (downHeld)  { moveDir = Vector2.down;  return; }
    }
    
    /*
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        
        if (inputVec.sqrMagnitude > 0.01f)
            lastAimDir = inputVec.normalized;

    }
    */



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Monster")) 
            return;
        var monster = collision.collider.GetComponentInParent<Monster>();
        if(monster==null)return;
        health -= monster.monsterDamage; //<- 몬스터데미지만큼 체력 감소
        Debug.Log($"플레이어 피격, 남은 체력 {health}");
        AudioManager.instance.PlaySfx(AudioManager.Sfx.PlayerHit);

        if (health > 0)
        {//살아있음

        }
        else // 사망
        {
            Respawn();
        }
    }    
    void Respawn() 
    { 
         Init(); // 플레이어 초기 설정
         AudioManager.instance.PlaySfx(AudioManager.Sfx.PlayerDead);

    }


    public void ApplyPlayerInfo(PlayerInfo info)
    {
        playerDamage = info.playerDamage;
        speed = info.speed;
        maxHealth = info.maxHealth;
        attackTimer = info.attackTimer;
        cooltimeTimer = info.cooltimeTimer;
    }

[SerializeField] float swingArc = 120f; 
[SerializeField] float swingDuration = 0.15f;  // 회전 시간 
[SerializeField] float hitActiveTime = 0.25f;  // 타격 판정 켜지는 시간(선택)

IEnumerator Attack()
{
    canAttack = false;

    weaponPivot.gameObject.SetActive(true);
    weapon.SetActive(true);
    AudioManager.instance.PlaySfx(AudioManager.Sfx.SwordAttack);

    float centerZ = Mathf.Atan2(lastAimDir.y, lastAimDir.x) * Mathf.Rad2Deg;
    float half = swingArc * 0.5f;
    float startZ = centerZ + half;
    float endZ   = centerZ - half;

    float t = 0f;
    while (t < swingDuration)
    {
        t += Time.deltaTime;
        float p = Mathf.Clamp01(t / swingDuration);
        float z = Mathf.LerpAngle(startZ, endZ, p);
        weaponPivot.localRotation = Quaternion.Euler(0, 0, z);
        yield return null;
    }

    // (선택) 판정 더 빨리 끄고 싶으면
    weapon.SetActive(false);
    weaponPivot.localRotation = Quaternion.identity;
    weaponPivot.gameObject.SetActive(false);

    yield return new WaitForSeconds(cooltimeTimer);
    canAttack = true;
}
    void OnAttack(InputValue value)
    {
       
        if (value.isPressed)
        {
            if (!canAttack)
                return;
            else
            {
                Debug.Log("공격");
                StartCoroutine(Attack());
                
                Debug.Log("공격완료");
            }
            
            
           
        }

    }
}

