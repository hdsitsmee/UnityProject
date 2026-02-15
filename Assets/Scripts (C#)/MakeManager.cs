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
        if (GameManager.instance == null) return;

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
            // 해금 레벨이 되었는가? (내 레벨 >= 재료 해금 레벨)
            if (myLevel >= data.unlockLevel)
            {
                // 버튼 생성!
                GameObject go = Instantiate(buttonPrefab, buttonContainer);
                IngredientButton btnScript = go.GetComponent<IngredientButton>();

                // 데이터 주입
                btnScript.Setup(data);

                // 관리 목록에 등록 (나중에 색깔 바꾸려고)
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
        int score = 0; 
        string message = "";

        foreach (string required in recipe.requiredIngredients)
        {
            if (currentIngredients.Contains(required)) matchCount++;
        }

        bool isSuccess = (matchCount == recipe.requiredIngredients.Length && currentIngredients.Count == recipe.requiredIngredients.Length);

        if (isSuccess)
        {
            Debug.Log("성공! 완벽한 음료입니다.");
            
            GameManager.AddMoney(500); // 돈 증가
            UpdateMoneyUI(); // ★ [추가] 돈이 올랐으니 화면도 갱신!

            if (GameManager.instance != null) GameManager.instance.GainExp(10); 

            score = 30; 
            recipe.hasMade = true; 
        }
        else
        {
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
        if (GameManager.instance != null)
        {
            GameManager.instance.currentOrderName = "";
            GameManager.instance.currentGuest = null; 
        }
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