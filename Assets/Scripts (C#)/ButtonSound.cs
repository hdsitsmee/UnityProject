using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound; //이 버튼을 누를 때 소리

    void Start()
    {
        //버튼 컴포넌트를 찾아서 클릭 기능을 자동으로 연결
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        //소리 재생 요청
        if (SoundManager.instance != null && clickSound != null)
        {
            SoundManager.instance.PlaySFX(clickSound);
        }
    }
}