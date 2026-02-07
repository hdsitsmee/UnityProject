using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionSlot : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public GameObject lockCover; //잠금 화면

    //데이터를 받아서 화면을 갱신하는 함수
    public void SetData(string name, Sprite sprite, bool isUnlocked)
    {
        Debug.Log($"슬롯 데이터 수신 - 이름: {name}, 이미지 존재여부: {(sprite != null ? "있음" : "없음")}");
        nameText.text = isUnlocked ? name : "???";

    if (sprite != null)
    {
        iconImage.sprite = sprite;
        iconImage.color = isUnlocked ? Color.white : Color.black;
    }

    lockCover.SetActive(!isUnlocked);
    }
}
