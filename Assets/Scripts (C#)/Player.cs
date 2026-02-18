using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public GameObject attackArea; 
    public GameObject weapon;
    [Header("플레이어 기본 정보")]
    public PlayerInfo info;

   

    [Header("공격 관련 스텟")]
    public float playerDamage;
    public float speed;
    public float health;


    
    public float attackTimer;//weapon 지속시간
    public float cooltimeTimer; // 공격 쿨타임 
    public Vector2 inputVec;
    bool canAttack;
    public float maxHealth;
    public Transform spawnPoint;
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
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }
    private void Update()
    {
       
        if (Time.timeScale == 0f) return;
    }
    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;
        if (inputVec.x != 0) 
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
  


    private void OnTriggerEnter2D(Collider2D collision) // ���Ϳ� ���� ü�� ���� �� ���
    {
        if (!collision.CompareTag("Monster")) 
            return;
        var monster = collision.GetComponentInParent<Monster>();
        health -= monster.monsterDamage; //<- 몬스터데미지만큼 체력 감소
        Debug.Log($"플레이어 피격, 남은 체력 {health}");

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
        canAttack = false;
        weapon.SetActive(true);
        Debug.Log("무기 활성화");
        yield return new WaitForSeconds(attackTimer);
        weapon.SetActive(false);
        Debug.Log("무기 비활성화");
        yield return new WaitForSeconds(cooltimeTimer);
        canAttack = true;
        Debug.Log("다시 공격 가능");

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

