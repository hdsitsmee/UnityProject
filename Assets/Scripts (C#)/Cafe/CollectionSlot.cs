using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionSlot : MonoBehaviour
{
    public Image iconImage; //아이템 사진
    public TMP_Text nameText; // 이름
    public GameObject lockPanel; // 잠금 표시 (자물쇠 이미지 등)

    // 데이터를 받아서 슬롯을 세팅
    public void SetSlot(string name, Sprite sprite, bool isUnlocked)
    {
        nameText.text = name;

        if (isUnlocked)
        {
            //해금
            iconImage.sprite = sprite;
            iconImage.color = Color.white; 
            lockPanel.SetActive(false);
            nameText.text = name;
        }
        else
        {
            // 잠김
            //iconImage.sprite = sprite;
            iconImage.color = Color.black;
            lockPanel.SetActive(true);
            nameText.text = "???";
        }
    }
}
