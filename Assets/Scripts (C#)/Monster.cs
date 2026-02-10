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

    [Header("아이템 드롭 설정 (선영)")]
    public ItemDatabase itemDatabase; 
    public InventoryData playerInventory; 

    [HideInInspector] public MonsterSpawner spawner;

    private float speed;
    private float changeDirIntervalMin;
    private float changeDirIntervalMax;

    Rigidbody2D rigidbody;

    Vector2 dir;
    float timer;

    // "게임플레이로 죽어서" 비활성화되는 경우만 리스폰 예약
    bool diedByGameplay;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    void OnEnable()
    {
        diedByGameplay = false; // 재사용될 때 초기화
        RandomDirection();
        ResetTimer();
    }

    void FixedUpdate()
    {
        Vector2 current = rigidbody.position;
        Vector2 nextPos = current + dir * speed * Time.fixedDeltaTime;

        if (floorTilemap != null)
        {
            Vector3Int cell = floorTilemap.WorldToCell(nextPos);
            if (!floorTilemap.HasTile(cell))
            {
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

        // 임시: 부딪히면 체력 감소
        health --;

        if (health <= 0)
        {
            Die();    
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Weapon"))
            return;

        health -= collision.GetComponent<Player>().playerDamage;
        if (health <= 0f)
            Die();
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
        if (!gameObject.activeSelf) return; // 중복 호출 방지

        diedByGameplay = true;             // "죽음" 표시

        if (itemDatabase != null && playerInventory != null)
        {
            Item droppedItem = itemDatabase.GetRandomItemByLevel(this.level);

            // droppedItem이 null이 아닐 때만 아래 코드 실행!
            if (droppedItem != null)
            {
                playerInventory.AddItem(droppedItem);

                // 팝업 인스턴스도 비어있는지 꼭 확인
                if (AcquisitionPopup.instance != null)
                {
                    AcquisitionPopup.instance.ShowMessage(droppedItem.itemName);
                }
                else
                {
                    Debug.LogError("AcquisitionPopup 인스턴스가 씬에 없습니다!");
                }
            }
            else
            {
                Debug.LogWarning(this.level + "레벨에 해당하는 아이템을 DB에서 찾을 수 없습니다.");
            }
        }

        // 여기서 바로 리스폰 예약을 걸고
        if (spawner != null)
            spawner.RequestRespawnOne();

        // 풀로 반환
        gameObject.SetActive(false);
    }

}
