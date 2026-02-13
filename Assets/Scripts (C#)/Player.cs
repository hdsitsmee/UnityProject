using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public GameObject attackArea; // ���� ����
    public GameObject weapon;
    [Header("������ ����")]
    public PlayerInfo info;

   

    [Header("��Ÿ�� ����(������ ����ϴ� ��)")]
    public float playerDamage;
    public float speed;
    public float health;


    
    public float attackTimer;//weapon �ֵθ��� �ð�
    public float cooltimeTimer; // ���� ��Ÿ��
    public Vector2 inputVec;
    bool canAttack;
    public float maxHealth;
    public Transform spawnPoint;
    void Init()
    {
        ApplyPlayerInfo(info);
     
        transform.position = spawnPoint.position; // ����������Ʈ���� ����
        health = maxHealth; // ü�� �ִ�ġ�� ����
        rigid.linearVelocity = Vector2.zero; // �ӵ� ����
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
    private void FixedUpdate() //�⺻ �̵�
    {
        if (Time.timeScale == 0f) return;
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }
    private void Update()
    {
        //if(cooltimeTimer > 0f) // ���� ��Ÿ���� ���Ҵٸ�
        //{
        //    cooltimeTimer -= Time.deltaTime; // ��Ÿ�� ����
        //}
        //if(isAttack) // ���� ������ ���¶�� 
        //{
        //    attackTimer -= Time.deltaTime; //�������ӽð�
        //    if(attackTimer <= 0)
        //    {
        //        attackArea.SetActive(false);
        //        isAttack = false;

        //    }
        //}
        if (Time.timeScale == 0f) return;
    }
    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;
        if (inputVec.x != 0) // �ȱ�
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
        health -= monster.monsterDamage; //<- ������ �����ϰ���
        Debug.Log("�÷��̾���ݹ���");

        if (health > 0)
        {// �������

        }
        else // ���ó�� �� ������
        {
            Respawn();
        }
    }    
    void Respawn() 
    { 
         Init(); // �ʱ� �������� ����
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
        Debug.Log("���� Ȱ��ȭ");
        yield return new WaitForSeconds(attackTimer);
        weapon.SetActive(false);
        Debug.Log("�����Ȱ��ȭ");
    }
    void OnAttack(InputValue value)
    {
       
        if (value.isPressed)
        {
            if (!canAttack)
                return;
            else
            {
                Debug.Log("����");
                StartCoroutine(Attack());
                canAttack = false;
                Debug.Log("��Ÿ�ӽ���");
            }
            
            
           
        }

    }
}

