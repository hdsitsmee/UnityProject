using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockPopupUI : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text levelText;
    public TMP_Text guestText;
    public TMP_Text drinkText;

    [Header("Icon Rows")]
    public Transform guestIconsParent;
    public Transform drinkIconsParent;
    public GameObject iconPrefab;

    public void Show(int level, List<string> newGuests, List<Sprite> newGuestsIcons,
        List<string> newDrinks, List<Sprite> newDrinksIcons
    )
    {
        Debug.Log($"Show: guestParentNull={guestIconsParent == null}, drinkParentNull={drinkIconsParent == null}, prefabNull={iconPrefab == null}, gCount={(newGuestsIcons == null ? -1 : newGuestsIcons.Count)}, dCount={(newDrinksIcons == null ? -1 : newDrinksIcons.Count)}");
        // 1) 텍스트 추가
        if (levelText != null)
            levelText.text = $"Lv {level}!";

        if (guestText != null)
            guestText.text = $"New Guest: {string.Join("\n, ", newGuests)}";

        if (drinkText != null)
            drinkText.text = $"New Recipe: {string.Join("\n, ", newDrinks)}";

        // 2) 아이콘 줄 초기화
        ClearChildren(guestIconsParent);
        ClearChildren(drinkIconsParent);

        // 3) 손님 아이콘 생성
        if (newGuestsIcons != null)
        {
            foreach (var sp in newGuestsIcons)
                CreateIcon(guestIconsParent, sp);
        }

        // 4) 음료 아이콘 생성
        if (newDrinksIcons != null)
        {
            foreach (var sp in newDrinksIcons)
                CreateIcon(drinkIconsParent, sp);
        }
    }

    void CreateIcon(Transform parent, Sprite sp)
    {
        if (parent == null || iconPrefab == null) return;

        var go = Instantiate(iconPrefab, parent);
        var img = go.GetComponent<Image>();
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        if (img == null) return;

        img.sprite = sp;
        img.color = Color.white;
    }

    // 기존 아이콘 초기화 함수
    void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
}
