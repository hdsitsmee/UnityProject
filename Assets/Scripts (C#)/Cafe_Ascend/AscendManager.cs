using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AscendManager : MonoBehaviour
{
    [Header("# UI")]
    public GameObject ascendPopup; // 성불 확인 팝업 UI
    public GameObject storyUI;  // 스토리 UI
    public GameObject storyteller;
    public TMP_Text storytext;
    public Button close;

    [Header("#Runtime")]
    public Animator ghostAnimator; // 현재 유령 Animator
    public GameObject ascend;
    public string ascendTrigger="DoAscend"; // Animator Trigger 이름
    public float ascendAnimDuration = 2.5f; 
    bool isFlowRunning;

    static public AscendManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;

        ascendTrigger ??= "DoAscend";              // null이면 기본값
        if (ascendTrigger.Trim().Length == 0)      // 빈문자/공백이면 기본값
            ascendTrigger = "DoAscend";
    }
    // 1. 성불 조건 충족 시 호출 -> 성불 알림 팝업 띄우고 일시정지
    public IEnumerator StartAscend()
    {
        Debug.Log("[Ascend] A: coroutine start");
        while (GameManager.instance.isGamePaused)
        {
            Debug.Log("[Ascend] B: still paused... timeScale=" + Time.timeScale);
            yield return null;
        }

        Debug.Log("[Ascend] C: pause cleared! timeScale=" + Time.timeScale);

        yield return new WaitForSecondsRealtime(0.2f);

        Debug.Log("[Ascend] D: after 0.2s realtime delay");

        isFlowRunning = true;
        Debug.Log("[Ascend] E: isFlowRunning true");

        //애니메이터 가져오기
        var cg = GameManager.instance.currentGuest; 
        GameObject targetObj = null; 
        if (cg.ghostPrefab != null) { 
            string prefabName = cg.ghostPrefab.name; 
            targetObj = GuestManager.instance.pool.Find(g => g != null && g.name.Contains(prefabName)); 
        } targetObj.SetActive(true); 
        targetObj.transform.Find("Face").gameObject.SetActive(false); // 기본 이미지는 비활
        ascend = targetObj.transform.Find("Ascend").gameObject; // Ascend만 활성화
        ascend.SetActive(true); ghostAnimator = ascend.GetComponentInChildren<Animator>(); 
        if (ascendPopup != null) ascendPopup.SetActive(true); 
        yield return new WaitForSeconds(5f); 
    }

    // 팝업의 "취소" 버튼 OnClick에 연결
    /*public void CancelAscend()
    {
        if (!isFlowRunning) return;

        if (ascendPopup != null) ascendPopup.SetActive(false);
        if (inputBlocker != null) inputBlocker.SetActive(false);

        PauseGame(false);
        isFlowRunning = false;
    }*/

    // 팝업의 "성불하기/확인" 버튼 OnClick에 연결
    // 2. 성불 진행 : 애니메이션 진행
    public void ConfirmAscend()
    {
        if (!isFlowRunning) return;

        // 알림 팝업 닫기
        if (ascendPopup != null) ascendPopup.SetActive(false); 

        // 스토리 배경 띄우기
        if (storyUI != null) storyUI.SetActive(true);

        // 성불 애니메이션 재생
        StartCoroutine(AscendRoutine());
    }

    IEnumerator AscendRoutine()
    {
        // 성불 애니 트리거 On -> 애니메이션 재생
        if (ghostAnimator != null)
        {
            ghostAnimator.ResetTrigger(ascendTrigger);
            ascend.transform.SetAsLastSibling();
            ghostAnimator.SetTrigger(ascendTrigger);
        }

        // 애니 끝날 때까지 대기
        yield return new WaitForSecondsRealtime(ascendAnimDuration);

        // 성불 스토리 보여주기
        if (storyteller != null)
        {
            storyteller.SetActive(true);
        }

        if (storytext != null)
        {
            storytext.gameObject.SetActive(true);
            storytext.text = GameManager.instance.currentGuest.ascendedDialogue;
        }



        // 여기서 다음 손님/상태 전환 등 호출 가능
        // 예: GameManager.instance.OnGhostAscended();

        // isFlowRunning은 “스토리 닫힐 때” 풀어주는 게 보통 더 자연스러움
    }

    // 스토리 팝업 닫기 버튼 OnClick
    public void CloseStory()
    {
        if (storyUI != null) storyUI.SetActive(false);
        if (storyteller != null)
        {
            storyteller.SetActive(false);
        }

        if (storytext != null)
        {
            storytext.gameObject.SetActive(false);
            storytext.text = ""; 
        }
        if (ascend != null) ascend.SetActive(false);
        isFlowRunning = false;
        GameManager.instance.isAscendMode = false; // 성불 모드 종료 플래그

        // 다음 로직
        // 예: GuestManager.instance.SpawnNextGuest();
    }
}
