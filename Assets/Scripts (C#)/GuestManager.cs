using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

//인내심 게이지 : 인내심 끝나면 손님 화나면서 퇴장 (애니메이션은 추후 추가) -> 이건 여기서

//유령은 랜덤으로 나옴+플레이어 레벨에 맞게
public class GuestManager : MonoBehaviour
{
    public TMP_Text speechBubbleText; //말풍선 텍스트
    public GameObject guestImage; //유령 이미지 얘를 레벨 조건에 맞게 조절해야돼
    public Button makeButton;

    public Slider patienceSlider;
    public float patienceTime = 10f;

    Coroutine patience;
    //손님 퇴장 처리 변수
    bool guestActive;

    void Start()
    {
        //게임 시작할 때는 손님이 없으니 버튼을 끈다
        makeButton.interactable = false; 
        
        //유령 그림도 숨긴다
        guestImage.SetActive(false);
    }
    //손님 부르기 (= 손님 메뉴 주문)
    public void CallGuest()
    {
        //GameManager에 등록된 모든 레시피를 가져온다
        List<DrinkRecipe> recipes = GameManager.instance.allRecipes;

        if (recipes.Count > 0)
        {
            //랜덤으로 하나
            int randomIndex = Random.Range(0, recipes.Count);
            DrinkRecipe selectedMenu = recipes[randomIndex];

            //주문 내용을 GameManager에 저장
            GameManager.instance.currentOrderName = selectedMenu.drinkName;

            //화면에 손님과 말풍선
            guestImage.SetActive(true); //유령 등장
            speechBubbleText.text = selectedMenu.drinkName;

            Debug.Log(selectedMenu.drinkName);
            makeButton.interactable = true;//버튼 활성화

            //손님 인내심 시작
            StartPatience();
        }
        else
        {
            Debug.LogError("메뉴판(Recipes)이 비어있습니다! GameManager를 확인하세요.");
        }
    }
    void StartPatience()
    {
        // 이전 손님 코루틴(=인내심) 정리
        if (patience != null) 
            StopCoroutine(patience);
        
        if (!patienceSlider)
            return;
        //인내심 게이지 설정
        patienceSlider.value = 1f; //인내심 100%
        patienceSlider.gameObject.SetActive(true);

        patience = StartCoroutine(PatienceRoutine());
    }

    IEnumerator PatienceRoutine()
    {
        float t = 0f;

        while (t < patienceTime)
        {
            // 손님이 이미 나갔으면 종료
            if (!guestActive) 
                yield break; 
            //시간에 따른 인내심 감소
            t += Time.deltaTime;
            float normalized = 1f - (t / patienceTime); // 1 -> 0
            //게이지에 감소한 인내심 적용
            if (patienceSlider != null)
                patienceSlider.value = normalized;

            yield return null;
        }

        // 인내심 0: 손님 화나서 퇴장
        GuestAngryLeave();
    }

    //인내심 끝난 손님 로직
    private void GuestAngryLeave()
    {
        guestActive = false; //화난 손님은 퇴장 처리
        makeButton.interactable = false; //버튼 비활성화
        speechBubbleText.text = "";
        GameManager.instance.currentOrderName = "";
        //퇴장 애니메이션으로 변경 예정
        Debug.Log("손님이 인내심 끝");
    }

    // (주문) 인내심 성공 손님 로직
    // 성공 함수 아직 호출 장소 미정
    public void CompleteOrder()
    {
        if (!guestActive) 
            return;
        guestActive = false; //성공 손님도 퇴장 처리

        if (patience != null) 
            StopCoroutine(patience);

        // 성공 처리
        makeButton.interactable = false;
        speechBubbleText.text = "";
        GameManager.instance.currentOrderName = "";

        guestImage.gameObject.SetActive(false);
        //인내심 게이지 제거
        if (patienceSlider != null) 
            patienceSlider.gameObject.SetActive(false);

        Debug.Log("주문 성공! 손님 퇴장");
    }

}
