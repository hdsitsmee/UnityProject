using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 구역 설정(Serializable)")]
    public SpawnAreaInfo spawnArea;

    [Header("몬스터 부모 오브젝트")]
    public Transform monstersParent;

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
        SpawnUpToMax();
    }

    public void SpawnUpToMax()
    {
        if (spawnArea == null || spawnArea.monsterPrefab == null)
        {
            Debug.LogWarning($"[{name}] spawnArea/monsterPrefab이 비어있음");
            return;
        }
        if (monstersParent == null)
        {
            Debug.LogWarning($"[{name}] monstersParent가 비어있음");
            return;
        }

        for (int i = 0; i < spawnArea.maxSpawnCount; i++)
        {
            if (TryGetSpawnPosition(out Vector3 pos))
            {
                var obj = Instantiate(spawnArea.monsterPrefab, pos, Quaternion.identity, monstersParent);

                // 몬스터 스탯 주입
                var m = obj.GetComponent<Monster>();
                if (m != null)
                {
                    m.ApplyMonsterInfo(spawnArea.monsterData);
                }
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
