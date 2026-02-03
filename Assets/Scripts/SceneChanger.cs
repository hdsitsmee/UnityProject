using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 메인 화면으로 이동
    public void ChangeToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    // 음료 제조 화면으로 이동
    public void ChangeToMake()
    {
        SceneManager.LoadScene("MakeScene");
    }

    // 던전 화면으로 이동
    
}
