using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 구역(=몬스터 레벨)")]
    public int areaLevel = 1; // 예: 1이면 1레벨 구역

    [Header("스폰할 몬스터 지정 및 최대 마릿수")]
    public GameObject monsterPrefab;
    public int maxCount = 5;

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
        for (int i = 0; i < maxCount; i++)
        {
            if (TryGetSpawnPosition(out Vector3 pos))
            {
                var obj = Instantiate(monsterPrefab, pos, Quaternion.identity, monstersParent);
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

            // 벽과 겹치면 다시 위치 잡기
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

