using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; // ★ TextMeshPro 사용을 위해 필수!

// 버튼과 재료 이름을 연결해주는 구조체
[System.Serializable]
public struct IngredientButtonMapping
{
    public string ingredientName; // 재료 이름 (예: "Milk")
    public Image buttonImage;     // 버튼의 이미지 컴포넌트
}

public class MakeManager : MonoBehaviour
{
    public static MakeManager instance;

    [Header("UI & Buttons")]
    public Button finishButton;
    public TMP_Text moneyText; // ★ [추가] 돈 표시할 텍스트
    public Transform buttonContainer;
    public GameObject buttonPrefab;

    [Header("Colors")]
    public Color selectedColor = Color.green;      
    public Color normalColor = Color.white;        

    [Header("Nirvana System")]
    public Slider nirvanaSlider; 
    public GameObject resultPopup; 
    public TMP_Text resultText;

    [Header("Data")]
    public List<string> currentIngredients = new List<string>();
    public string currentOrderName = ""; 

    private Dictionary<string, IngredientButton> spawnedButtons = new Dictionary<string, IngredientButton>();

    private bool isTutorialMode = false;
    private DrinkData targetRecipe; //[변경] 변수 형식 DrinkRecipe -> DrinkData

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 1. 주문 확인
        if (GameManager.instance != null && GameManager.instance.currentOrderName != "")
        {
            currentOrderName = GameManager.instance.currentOrderName;
        }
        else
        {
            currentOrderName = "great water"; 
        }
        Debug.Log("제조 시작! 목표 메뉴: " + currentOrderName);

        // 2. 목표 레시피 가져오기
        if (GameManager.instance != null)
        {
            targetRecipe = GameManager.instance.GetRecipeByName(currentOrderName);
        }

        // 3. UI 초기화
        SpawnIngredientButtons();

        UpdateMoneyUI(); // ★ 시작할 때 현재 돈 표시
        CheckAndShowTutorial();
        CheckFinishCondition();
        
        if(resultPopup != null) resultPopup.SetActive(false); 
    }
    void SpawnIngredientButtons()
    {
        if (GameManager.instance == null || buttonContainer == null) return;

        // 기존에 있는 버튼들 싹 지우기 (초기화)
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        spawnedButtons.Clear();

        int myLevel = GameManager.level;

        // 게임매니저에 등록된 모든 재료를 검사
        foreach (var data in GameManager.instance.allIngredients)
        {
            int count = GameManager.instance.playerInventory.GetItemCount(data.ingredientName);
            Debug.Log($"[재료 연동 테스트] 카페 재료 이름: {data.ingredientName} / 인벤토리에서 찾은 개수: {count}개");

            //if (myLevel >= data.unlockLevel && count > 0)
            if (myLevel >= data.unlockLevel)
            {
                GameObject go = Instantiate(buttonPrefab, buttonContainer);
                IngredientButton btnScript = go.GetComponent<IngredientButton>();

                btnScript.Setup(data);
                spawnedButtons.Add(data.ingredientName, btnScript);
            }
        }
    }
    public void OnIngredientClicked(string ingredientName, IngredientButton btnScript)
    {
        // 1. 소리 재생
        if (SoundManager.instance != null)
        {
            var data = GameManager.instance.GetIngredientData(ingredientName);
            if (data != null && data.soundEffect != null)
                SoundManager.instance.PlaySFX(data.soundEffect); // 고유 소리
            else
                SoundManager.instance.PlaySFX(null); // 혹은 기본 클릭음
        }

        // 2. 선택/해제 로직
        if (currentIngredients.Contains(ingredientName))
        {
            currentIngredients.Remove(ingredientName);
            
            // 색상 복귀 (튜토리얼 중이면 튜토리얼 색상, 아니면 흰색)
            if (isTutorialMode)
            {
                bool isRequired = IsIngredientRequired(ingredientName);
                btnScript.SetTutorialAnimation(isRequired); 
                btnScript.SetColor(normalColor);
            }
            else
            {
                btnScript.SetColor(normalColor);
            }
        }
        else
        {
            currentIngredients.Add(ingredientName);
            btnScript.SetTutorialAnimation(false); 
            btnScript.SetColor(selectedColor);
        }

        CheckFinishCondition();
    }

    // ★ 돈 UI 갱신 함수
    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            // GameManager의 정적 변수 money를 가져와서 표시
            moneyText.text = "Money: " + GameManager.money;
        }
    }

    void CheckAndShowTutorial()
    {
        if (targetRecipe == null) return;

        if (!targetRecipe.hasMade)
        {
            isTutorialMode = true;
            
            foreach (var pair in spawnedButtons)
            {
                string name = pair.Key;
                IngredientButton btn = pair.Value;

                bool isRequired = IsIngredientRequired(name);
                
                btn.SetTutorialAnimation(isRequired);
            }
        }
        else
        {
            isTutorialMode = false;
            foreach(var btn in spawnedButtons.Values)
            {
                btn.SetTutorialAnimation(false);
                btn.SetColor(normalColor); 
            }
        }
    }

    bool IsIngredientRequired(string ingredientName)
    {
        if (targetRecipe == null) return false;
        foreach (string req in targetRecipe.requiredIngredients)
        {
            if (req == ingredientName) return true;
        }
        return false;
    }

    void PrintCurrentStatus()
    {
        Debug.Log("현재 믹스: [ " + string.Join(", ", currentIngredients) + " ]");
    }

    void CheckFinishCondition()
    {
        if (finishButton != null)
            finishButton.interactable = (currentIngredients.Count > 0);
    }

    public void OnClickFinish()
    {
        if (targetRecipe != null)
        {
            CheckResult(targetRecipe);
        }
        else
        {
            Debug.LogError("레시피 정보가 없어 채점할 수 없습니다.");
            StartCoroutine(WaitAndGoMain());
        }
    }

    void CheckResult(DrinkData recipe)
    {
        int matchCount = 0;
        float score = 0; 
        string message = "";

        foreach (string required in recipe.requiredIngredients)
        {
            if (currentIngredients.Contains(required)) matchCount++;
        }

        bool isSuccess = (matchCount == recipe.requiredIngredients.Length && currentIngredients.Count == recipe.requiredIngredients.Length);

        // 🥨 [추가] 음료별 만족도 차등 부여
        // 상승도 : 음료 번호 * 맞춘 재료/올바른 재료 * 고정 상수 20
        int DrinkNum = GameManager.instance.recipebook.allRecipes.IndexOf(GameManager.instance.currentDrink)+1;
        score = DrinkNum*((float)matchCount / recipe.requiredIngredients.Length); // 0.0 ~ 1.0 사이의 점수
        score = Mathf.Round(score * 20); // 20 곱하고 반올림

        // 🥨 [추가] 제조 실패 -> 만족도 차등 감소
        if (!isSuccess)
        {
            score = -score;
        }

        if (isSuccess)
        {
            message = "Great!"; // 한글쓰면 팝업창이 깨지는 현상 있어서 일단 영어로 바꿨습니다 ㅠㅠ
            Debug.Log(message);
            
            GameManager.AddMoney(500); // 돈 증가
            UpdateMoneyUI(); // 돈이 올랐으니 화면도 갱신

            if (GameManager.instance != null) GameManager.instance.GainExp(100);
            
            recipe.hasMade = true; 
            foreach (string usedIng in currentIngredients)
            {
                GameManager.instance.playerInventory.ConsumeItem(usedIng, 1);
            }
        }
        else
        {
            message = "No : Check Recipe";
            Debug.Log("실패...");
            Debug.Log($"필요: {recipe.requiredIngredients.Length} / 맞춤: {matchCount}");
        }

        if (resultPopup != null)
        {
            // 1. 일단 팝업창을 무조건 띄운다
            resultPopup.transform.SetAsLastSibling(); // 맨 앞으로 가져오기
            resultPopup.SetActive(true); 
            
            // 2. 텍스트 표시
            if (resultText != null) resultText.text = message;

            // 3. 슬라이더 데이터 반영 (손님 정보가 있을 때만)
            if (GameManager.instance != null && GameManager.instance.currentGuest != null)
            {
                GuestData guest = GameManager.instance.currentGuest;
                GameManager.instance.UpdateGuestSatisfaction(guest.guestName, score); 
                
                if (nirvanaSlider != null)
                {
                    nirvanaSlider.maxValue = guest.maxSatisfaction;
                    nirvanaSlider.value = guest.currentSatisfaction;
                }
            }
            else
            {
                // 손님 정보가 없으면 슬라이더를 0으로 두거나 숨김 처리
                Debug.LogWarning("손님 데이터(currentGuest)가 없어서 성불 수치를 반영하지 못했습니다.");
                if (nirvanaSlider != null) nirvanaSlider.value = 0;
            }
            
        }
        // 4. 게임 매니저에 결과 전달
        // 🥨 [중요] 게스트 매니저에서 채점, 결과 반영 중복 로직 삭제
        // (제조 씬에서 채점 후 결과 전달 -> 결과 씬에서 게스트 매니저가 받아서 메인 화면 반응 로직 수행)
        if (GameManager.instance != null)
        {
            GameManager.instance.lastResultSuccess = isSuccess; // 게임 매니저에 성공 여부 전달
            GuestData gd = GameManager.instance.currentGuest;
            GameManager.instance.reactDialogue = isSuccess ?  gd.happyDialogue: gd.angryDialogue; // 게임 매니저에 반응 텍스트 전달
            if (!GameManager.instance.isAscendMode) // 성불 모드 아닐 때만 React 진입
                GameManager.instance.reactPending = true; // 씬 돌아왔을 때 React 진입 플래그
            GameManager.instance.StopOrderTimer();
        }
        StartCoroutine(WaitAndGoMain());
    }

    void ResetAllButtonColors()
    {
        foreach (var btn in spawnedButtons.Values)
        {
            btn.SetColor(normalColor);
            btn.SetTutorialAnimation(false);
        }
    }

    void GoToMain()
    {
        // 🥨 [중요] 주문 데이터 초기화 x
        // (제조 -> 메인 이동 시 주문 데이터 유지하면서 결과 화면에서 반영하기 위해)
        /*if (GameManager.instance != null)
        {
            GameManager.instance.currentOrderName = "";
            GameManager.instance.currentGuest = null; 
        }*/
        SceneManager.LoadScene("MainScene");
    }

    System.Collections.IEnumerator WaitAndGoMain()
    {
        Debug.Log("4초 뒤 메인으로 이동");
        yield return new WaitForSecondsRealtime(4.0f);
        Debug.Log("메인으로 이동");
        GoToMain();
    }
}