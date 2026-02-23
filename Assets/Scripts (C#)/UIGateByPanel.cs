using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGateByPanel : MonoBehaviour
{
    TMP_Text speechBubbleText;
    GameObject orderBullon;
    Slider patienceSlider;
    //🥨 [추가] 기존화면 말풍선 출현 여부
    bool snapOrder;

    void Awake()
    {
        speechBubbleText = GuestManager.instance?.speechBubbleText;
        orderBullon = GuestManager.instance?.OrderBullon;
        patienceSlider = GuestManager.instance?.patienceSlider;
    }

    void OnEnable()
    {
        // 1. 도감 On -> 정지 플래그 설정 = 게스트 매니저 흐름 일시정지
        GameManager.instance.SetPause(true);
        // 🥨 [추가] 1-1. 화면 전환 직전 말풍선이 있었는지 저장
        snapOrder = orderBullon != null && orderBullon.activeSelf;
        // 2. 유령 오브젝트 비활
        if (GameManager.instance.currentGuest != null && GameManager.instance.currentGuest.ghostPrefab != null)
        {
            var cg = GameManager.instance.currentGuest;
            GameObject targetObj = null;
            if (cg.ghostPrefab != null)
            {
                string prefabName = cg.ghostPrefab.name;
                targetObj = GuestManager.instance.pool.Find(g => g != null && g.name.Contains(prefabName));
            }
            targetObj.SetActive(false);
        }
    
        ApplyGate();     // UI 숨김
    }

    void OnDisable()
    {
        GameManager.instance.SetPause(false); // 도감 닫히면 게임 진행 재개
        if (GameManager.instance.currentGuest != null)
        {
            var cg = GameManager.instance.currentGuest;
            GameObject targetObj = null;
            if (cg.ghostPrefab != null)
            {
                string prefabName = cg.ghostPrefab.name;
                targetObj = GuestManager.instance.pool.Find(g => g != null && g.name.Contains(prefabName));
            }
            targetObj.SetActive(true);
        }
        RestoreSnapshot();   // 직전 화면 저장한 그대로 복구
    }


    void RestoreSnapshot()
    {
        /*if (!hasSnapshot) return;
        hasSnapshot = false;*/

        // 🥨 [추가] 이전 화면에 말풍선이 없었다면 복구 x
        if (orderBullon != null)
            orderBullon.SetActive(snapOrder);

        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(true);
        }

        if (patienceSlider != null)
        {
            PatienceUI(true);
        }
    }

    void ApplyGate()
    {
        // 도감 ON -> 강제 숨김
        if (orderBullon != null) orderBullon.SetActive(false);

        if (speechBubbleText != null)
        {
            speechBubbleText.gameObject.SetActive(false);
        }

        if (patienceSlider != null)
            PatienceUI(false);
    }

    void PatienceUI(bool flag)
    {
        foreach (Transform child in patienceSlider.transform)
        {
            child.gameObject.SetActive(flag);
        }
    }
      
}
