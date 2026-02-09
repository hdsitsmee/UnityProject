using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Monster : MonoBehaviour
{
    [Header("몬스터 정보")]
    public int level;
    public float maxHealth;
    public float monsterDamage; 
    [Header("몬스터 개별 체력(런타임)")]
    public float health;

    [Header("바닥 타일맵(이동 구역)")]
    public Tilemap floorTilemap;
    // 몬스터마다 이동 구역 Tilemap_Floor 지정함

    private float speed;
    private float changeDirIntervalMin;
    private float changeDirIntervalMax;

    Rigidbody2D rigidbody;
    

    Vector2 dir;
    float timer;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        
        health = maxHealth; // ApplyMonsterInfo가 Start 전에 호출되면 이 값이 덮어써짐
        
    }

    void OnEnable()
    {
        RandomDirection();
        ResetTimer();
    }

    void FixedUpdate()
    {
        Vector2 current = rigidbody.position;
        Vector2 nextPos = current + dir * speed * Time.fixedDeltaTime;

        //바닥 타일맵이 지정되어 있으면: 다음 위치가 바닥 위인지 검사
        if (floorTilemap != null)
        {
            Vector3Int cell = floorTilemap.WorldToCell(nextPos); //다음 이동 자리
            if (!floorTilemap.HasTile(cell))
            {
                //바닥이 아니면 이동하지 말고 방향만 바꾸기
                RandomDirection();
                ResetTimer();
                return;
            }
        }

        rigidbody.MovePosition(nextPos);

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            RandomDirection();
            ResetTimer();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        RandomDirection();
        ResetTimer();

        // 임시: 벽에 부딪히면 체력 감소
        health -= 0.1f;

        
    
       
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Weapon"))
            return;
        health -= collision.GetComponent<Player>().playerDamage;
        if (health > 0)
            return;

        else
        {
            Die();
        }
    }

    void RandomDirection()
    {
        int r = Random.Range(0, 4);
        dir = r switch
        {
            0 => Vector2.up,
            1 => Vector2.down,
            2 => Vector2.left,
            _ => Vector2.right,
        };
    }

    void ResetTimer()
    {
        timer = Random.Range(changeDirIntervalMin, changeDirIntervalMax);
    }

    public void ApplyMonsterInfo(MonsterInfo info)
    {
        level = info.level;
        maxHealth = info.maxHealth;
        health = info.maxHealth;
        monsterDamage = info.monsterDamage;
        speed = info.speed;
        changeDirIntervalMax = info.changeDirIntervalMax;
        changeDirIntervalMin = info.changeDirIntervalMin;
        floorTilemap = info.moveAreaTilemap;
    }

    
    void Die()
    {
        gameObject.SetActive(false);
    }
}
