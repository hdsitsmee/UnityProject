using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("스피커 설정")]
    public AudioSource bgmPlayer;//배경음악용 스피커
    public AudioSource sfxPlayer;//효과음용 스피커

    [Header("배경음악 파일")]
    public AudioClip cafeBgm;//카페 배경음악
    public AudioClip levelUpSound;//레벨업 효과음

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //게임 시작하자마자 카페 재생
        PlayBGM(cafeBgm);
    }

    //배경음악 재생 함수
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        bgmPlayer.clip = clip;
        bgmPlayer.loop = true; //무한 반복
        bgmPlayer.Play();
    }

    //효과음 재생 함수
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxPlayer.PlayOneShot(clip); 
    }
}