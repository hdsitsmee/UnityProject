using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public GameObject attackArea; // 공격 범위
    
    [Header("데이터 에셋")]
    public PlayerInfo info;

   

    [Header("런타임 스탯(실제로 사용하는 값)")]
    public float playerDamage;
    public float speed;
    public float health;


    //bool canAttack; //공격 가능 상태 판단
    //bool isAttack; // 공격 중인 상태 판단
    //public float attackTimer;//공격 시간 계산
    //public float cooltimeTimer; // 공격 쿨타임
    public Vector2 inputVec;
    
    public float maxHealth;
    public Transform spawnPoint;
    void Init()
    {
        ApplyPlayerInfo(info);
     
        transform.position = spawnPoint.position; // 리스폰포인트에서 시작
        health = maxHealth; // 체력 최대치로 조정
        rigid.linearVelocity = Vector2.zero; // 속도 조절
        //cooltimeTimer = 1.5f; //공격 쿨타임 설정
        //attackTimer = 0.5f; //공격 시간 -> 사실 필요한지 잘 모르겠음
        gameObject.SetActive(true); 
    }
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Monster monster;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate() //기본 이동
    {
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }
    private void Update()
    {
        //if(cooltimeTimer > 0f) // 공격 쿨타임이 남았다면
        //{
        //    cooltimeTimer -= Time.deltaTime; // 쿨타임 감소
        //}
        //if(isAttack) // 공격 가능한 상태라면 
        //{
        //    attackTimer -= Time.deltaTime; //공격지속시간
        //    if(attackTimer <= 0)
        //    {
        //        attackArea.SetActive(false);
        //        isAttack = false;

        //    }
        //}
    }
    private void LateUpdate()
    {
        if (inputVec.x != 0) // 걷기
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
    //void OnAttack(InputValue value)
    //{
    //    canAttack = !(cooltimeTimer > 0f) || !(isAttack);
    //    if (value.isPressed && canAttack)
    //    {
    //        monster.TakeDamage(10f); // 임시값
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision) // 몬스터에 의한 체력 감소 및 사망
    {
        if (!collision.CompareTag("Monster")) //아직 태그 없어서 추가해야함
            return;
        
        health -= monster.monsterDamage; //<- 데미지 선언하고서

        if (health > 0)
        {// 살아있음

        }
        else // 사망처리 후 리스폰
        {
            Respawn();
        }
    }    
    void Respawn() 
    { 
         Init(); // 초기 스텟으로 설정
    }


    public void ApplyPlayerInfo(PlayerInfo info)
    {
        playerDamage = info.playerDamage;
        speed = info.speed;
        maxHealth = info.maxHealth;
}
}

