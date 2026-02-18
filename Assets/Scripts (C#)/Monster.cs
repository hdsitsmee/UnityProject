using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

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
    Animator anim;
    SpriteRenderer sr;
    Vector2 dir;
    float timer;

    // "게임플레이로 죽어서" 비활성화되는 경우만 리스폰 예약
    bool diedByGameplay;
    int facing; //0 1 2 정면 후면 측면
    bool isHitPlaying; //피격 
    float hitEffectTime=0.12f;

    Coroutine co;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

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
        HitEffectPlay(); //피격 효과. 이것도 임시!

        if (health <= 0)
        {
            Die();    
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Weapon"))
        {
            var player = collision.GetComponentInParent<Player>();
            if (player == null) return;

            health -= player.playerDamage;
            Debug.Log("몬스터가공격받음");

            //PlayHitAnim(); //Hit 애니메이션
            HitEffectPlay(); 
        }

       
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

        UpdateFacingByDir();
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
            Item droppedItem = itemDatabase.GetItemByMonsterLevel(this.level); 

            if (droppedItem != null)
            {
                playerInventory.AddItem(droppedItem);

                if (AcquisitionPopup.instance != null)
                {
                    AcquisitionPopup.instance.ShowMessage(droppedItem.itemName); 
                }
            }
        }

        // 여기서 바로 리스폰 예약을 걸고
        if (spawner != null)
            spawner.RequestRespawnOne();

        // 풀로 반환
        gameObject.SetActive(false);
    }

    void UpdateFacingByDir()
    {
        // dir은 up/down/left/right 중 하나
        if (dir.y > 0) facing = 1;          // Up = Back
        else if (dir.y < 0) facing = 0;     // Down = Front
        else facing = 2;                    // Left/Right = Side

        if (anim != null) anim.SetInteger("Facing", facing);

        // Side일 때만 좌우 플립
        if (sr != null)
        {
            if (facing == 2) sr.flipX = (dir.x < 0);
            else sr.flipX = false; // 앞/뒤는 flip 끔(원하면 유지해도 됨)
        }
    }

    /*
    void PlayHitAnim()
    {
        if (anim == null) return;

        // 너무 연타로 트리거가 계속 걸리면 정신없으니 짧게 잠금(선택)
        if (isHitPlaying) return;
        isHitPlaying = true;

        anim.SetTrigger("Hit");
        Invoke(nameof(UnlockHit), 0.1f); // 0.1~0.2 정도면 충분
    }

    void UnlockHit() => isHitPlaying = false;
    */

     public void HitEffectPlay()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Flash());
    }
    IEnumerator Flash()
    {
        var prev = new Color(1f,1f,1f,1f);
        sr.color = new Color(1f, 0.3f, 0.3f, 1f);
        yield return new WaitForSeconds(hitEffectTime);
        sr.color = prev;
        co = null;
    }
}
