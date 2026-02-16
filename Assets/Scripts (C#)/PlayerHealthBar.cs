using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("Refs")]
    public Slider slider2;            // Health/Canvas/Slider
    public GameObject slider2Root;    // 숨길 대상(보통 Slider 게임오브젝트)

    Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();

        if (slider2 == null)
            slider2 = GetComponentInChildren<Slider>(true);

        if (slider2Root == null && slider2 != null)
            slider2Root = slider2.gameObject;
    }

    void OnEnable()
    {
        Refresh(true); 
    }

    void LateUpdate()
    {
        Refresh(false);
    }

    void Refresh(bool force)
    {
        if (player == null || slider2 == null || slider2Root == null) return;

        float max = Mathf.Max(1f, player.maxHealth);
        float ratio = Mathf.Clamp01(player.health / max);

        // 슬라이더 반영
        slider2.value = ratio;
    }
}

