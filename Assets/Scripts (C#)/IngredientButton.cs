using UnityEngine;
using UnityEngine.UI;

public class IngredientButton : MonoBehaviour
{
    public Image targetImage;
    public Button btn;//버튼 컴포넌트

    private string myName;//내 재료 이름 (MakeManager에게 알려줄 용도)

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

    // 색깔 바꾸기 (선택됨/안됨/튜토리얼 강조 등)
    public void SetColor(Color color)
    {
        if (targetImage != null) targetImage.color = color;
    }
}
