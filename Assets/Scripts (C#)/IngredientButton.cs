using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngredientButton : MonoBehaviour
{
    public Image targetImage;
    public Button btn;//버튼 컴포넌트

    private string myName;//내 재료 이름 (MakeManager에게 알려줄 용도)
    private Coroutine animRoutine;

    //생성될 때 데이터를 받아서 세팅하는 함수
    public void Setup(IngredientData data)
    {
        myName = data.ingredientName;

        // 아이콘 설정
        if (data.icon != null) 
        {
            targetImage.sprite = data.icon;
            
            // (중요) 이미지 비율 원본대로 맞추기 (찌그러짐 방지)
            targetImage.preserveAspect = true; 
        }

        // 버튼 클릭 이벤트 연결
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            // 클릭되면 MakeManager에게 "나(myName) 눌렸어!" 하고 보고함
            MakeManager.instance.OnIngredientClicked(myName, this);
        });
    }
    IEnumerator BounceRoutine()
    {
        Vector3 originalScale = Vector3.one;
        while (true)
        {
            float scale = 1.0f + Mathf.PingPong(Time.time * 0.1f, 0.1f);
            
            transform.localScale = originalScale * scale;
            yield return null;
        }
    }
    public void SetTutorialAnimation(bool play)
    {
        if (play)
        {
            // 이미 돌고 있으면 중복 실행 방지
            if (animRoutine == null) 
                animRoutine = StartCoroutine(BounceRoutine());
        }
        else
        {
            // 애니메이션 정지 및 원상복구
            if (animRoutine != null) StopCoroutine(animRoutine);
            animRoutine = null;
            transform.localScale = Vector3.one; // 크기 초기화 (중요!)
        }
    }
    // 색깔 바꾸기 (선택됨/안됨/튜토리얼 강조 등)
    public void SetColor(Color color)
    {
        if (targetImage != null) targetImage.color = color;
    }
}
