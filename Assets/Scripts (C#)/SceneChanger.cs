using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 메인 화면으로 이동
    public void ChangeToMain()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayBGM(SoundManager.instance.cafeBgm);
        }
        SceneManager.LoadScene("MainScene");
    }

    // 음료 제조 화면으로 이동
    public void ChangeToMake()
    {
        SceneManager.LoadScene("MakeScene");
    }

    // 던전 화면으로 이동
    public void ChangeToDungeon()
    {
        if (GuestManager.instance != null)
        {
            GuestManager.instance.StopAllCoroutines();
        }
        if (GameManager.instance != null)
        {
            GameManager.instance.isGamePaused = false; 
        }
        if (SoundManager.instance != null)
        {
            SoundManager.instance.bgmPlayer.Stop();
        }
        SceneManager.LoadScene("DungeonScene");
    }
}
