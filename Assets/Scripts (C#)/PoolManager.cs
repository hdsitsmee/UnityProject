using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;
    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++)
            pools[i] = new List<GameObject>();
    }

    // 원하는 개수만큼 미리 만들어두기
    public void Prewarm(int index, int count, Transform parent = null)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = CreateNew(index, parent);
            obj.SetActive(false);
        }
    }

    GameObject CreateNew(int index, Transform parent)
    {
        var obj = Instantiate(prefabs[index], parent);
        pools[index].Add(obj);
        return obj;
    }

    public GameObject Get(int index, Transform parent = null)
    {
        for (int i = 0; i < pools[index].Count; i++)
        {
            var item = pools[index][i];
            if (!item.activeSelf)
            {
                if (parent != null) item.transform.SetParent(parent, false);
                item.SetActive(true);
                return item;
            }
        }

        // 풀에 남는 게 없으면 새로 만들어서라도 반환 (선택)
        var created = CreateNew(index, parent);
        created.SetActive(true);
        return created;
    }
}
