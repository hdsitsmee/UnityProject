using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public GameObject attackArea; // 공격 범위
    public GameObject weapon;
    [Header("데이터 에셋")]
    public PlayerInfo info;

   

    [Header("런타임 스탯(실제로 사용하는 값)")]
    public float playerDamage;
    public float speed;
    public float health;


    
    public float attackTimer;//weapon 휘두르는 시간
    public float cooltimeTimer; // 공격 쿨타임
    public Vector2 inputVec;
    bool canAttack;
    public float maxHealth;
    public Transform spawnPoint;
    void Init()
    {
        ApplyPlayerInfo(info);
     
        transform.position = spawnPoint.position; // 리스폰포인트에서 시작
        health = maxHealth; // 체력 최대치로 조정
        rigid.linearVelocity = Vector2.zero; // 속도 조절
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
    private void FixedUpdate() //기본 이동
    {
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }
    private void Update()
    {
        if (cooltimeTimer > 0f) // 공격 쿨타임이 남았다면
        {
            cooltimeTimer -= Time.deltaTime; // 쿨타임 감소
        }
        else
        {
            canAttack = true;
            
        }
            
        
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
  


    private void OnTriggerEnter2D(Collider2D collision) // 몬스터에 의한 체력 감소 및 사망
    {
        if (!collision.CompareTag("Monster")) 
            return;
        var monster = collision.GetComponentInParent<Monster>();
        health -= monster.monsterDamage; //<- 데미지 선언하고서
        Debug.Log("플레이어가공격받음");

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
        attackTimer = info.attackTimer;
        cooltimeTimer = info.cooltimeTimer;
    }


    IEnumerator Attack()
    {
        weapon.SetActive(true);
        Debug.Log("무기 활성화");
        yield return new WaitForSeconds(attackTimer);
        weapon.SetActive(false);
        Debug.Log("무기비활성화");
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
                canAttack = false;
                Debug.Log("쿨타임시작");
            }
            
            
           
        }

    }
}

