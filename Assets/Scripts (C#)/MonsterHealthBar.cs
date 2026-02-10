using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBarUI : MonoBehaviour
{
    [Header("Refs")]
    public Slider slider;            // Health/Canvas/Slider
    public GameObject sliderRoot;    // 숨길 대상(보통 Slider 게임오브젝트)

    Monster monster;

    void Awake()
    {
        monster = GetComponentInParent<Monster>();

        if (slider == null)
            slider = GetComponentInChildren<Slider>(true);

        if (sliderRoot == null && slider != null)
            sliderRoot = slider.gameObject;
    }

    void OnEnable()
    {
        Refresh(true); // 시작 시 한 번 반영 (풀링 재사용 대비)
    }

    void LateUpdate()
    {
        Refresh(false);
    }

    void Refresh(bool force)
    {
        if (monster == null || slider == null || sliderRoot == null) return;

        float max = Mathf.Max(1f, monster.maxHealth);
        float ratio = Mathf.Clamp01(monster.health / max);

        // 슬라이더 반영
        slider.value = ratio;

        // 풀피면 숨기기
        bool isFull = ratio >= 0.999f;
        if (sliderRoot.activeSelf == isFull) // 상태 바뀔 때만 SetActive
            sliderRoot.SetActive(!isFull);
    }
}
