using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Transform weaponPivot;
    public GameObject attackArea; //불필요?
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
    float timer; //회전 공격에 사용
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
            float scaleX = Mathf.Abs(transform.localScale.x);
            spriter.flipX = inputVec.x < 0;
            float xRotation = inputVec.x < 0 ? -scaleX : scaleX;
            transform.localScale = new Vector3(xRotation, transform.localScale.y, transform.localScale.z);
        }
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
  


    void OnCollisionEnter2D(Collision2D collision) // ���Ϳ� ���� ü�� ���� �� ���
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
        weaponPivot.gameObject.SetActive(true);
        weapon.SetActive(true);
        Debug.Log("무기 활성화");
        AudioManager.instance.PlaySfx(AudioManager.Sfx.SwordAttack);
        
        timer = 0f;
        Quaternion startRotation = Quaternion.Euler(0, 0, 90f);  // 시작 각도 (위)
        Quaternion endRotation = Quaternion.Euler(0, 0, -90f);   // 끝 각도 (아래)

        while (timer < attackTimer)

        {

            timer += Time.deltaTime;
            float progress = timer / attackTimer; // 0에서 1까지 진행률

            // 시간에 따라 회전값 보간 (Lerp)

             weaponPivot.localRotation = Quaternion.Lerp(startRotation, endRotation, progress);



            yield return null; // 다음 프레임까지 대기

        }
        //yield return new WaitForSeconds(attackTimer);
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

