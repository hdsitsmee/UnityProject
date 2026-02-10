using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 설정(Serializable)")]
    public SpawnAreaInfo spawnArea;

    [Header("풀 매니저")]
    public PoolManager pool;
    public int poolIndex = 0;

    [Header("리스폰")]
    public float respawnInterval = 1.0f;

    [Header("기타")]
    public LayerMask wallLayerMask;
    public float overlapCheckRadius = 0.2f;
    public int maxTriesPerMonster = 50;

    BoxCollider2D area;
    

    void Awake()
    {
        area = GetComponent<BoxCollider2D>();
        area.isTrigger = true;
    }

    void Start()
    {
        // 1) 처음에 최대수량만큼 풀에 미리 만들어두기
        pool.Prewarm(poolIndex, spawnArea.maxSpawnCount, spawnArea.monstersParent);

        // 2) 시작 시 최대 스폰
        SpawnUpToMax();
    }

    public void RequestRespawnOne()
    {
        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnInterval);

        // 이미 최대수량이면 스폰 안 함 (죽은 사이에 다른 이유로 채워졌을 수 있음)
        if (GetActiveCount() >= spawnArea.maxSpawnCount)
            yield break;

        SpawnOne();
    }

    int GetActiveCount()
    {
        int count = 0;
        for (int i = 0; i < spawnArea.monstersParent.childCount; i++)
        {
            if (spawnArea.monstersParent.GetChild(i).gameObject.activeSelf)
                count++;
        }
        return count;
    }

    public void SpawnUpToMax()
    {
        for (int i = 0; i < spawnArea.maxSpawnCount; i++)
            SpawnOne();
    }

    void SpawnOne()
    {
        if (TryGetSpawnPosition(out Vector3 pos))
        {
            var obj = pool.Get(poolIndex, spawnArea.monstersParent);

            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;

            var m = obj.GetComponent<Monster>();
            if (m != null)
            {
                // Monster 쪽에 public MonsterSpawner spawner; 필드가 있음
                m.spawner = this;

                // 풀 재사용이므로 리스폰 때마다 다시 주입하는 게 안전
                m.ApplyMonsterInfo(spawnArea.monsterData);
            }
        }
    }

    bool TryGetSpawnPosition(out Vector3 pos)
    {
        Bounds b = area.bounds;

        for (int t = 0; t < maxTriesPerMonster; t++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            Vector3 p = new Vector3(x, y, 0f);

            if (wallLayerMask.value != 0)
            {
                if (Physics2D.OverlapCircle(p, overlapCheckRadius, wallLayerMask) != null)
                    continue;
            }

            pos = p;
            return true;
        }

        pos = default;
        return false;
    }
}
