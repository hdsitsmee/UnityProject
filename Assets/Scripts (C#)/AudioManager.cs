using UnityEngine;
using UnityEngine.PlayerLoop;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")] //배경음 
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")] //효과음 
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx{MonsterDead, MonsterHit, PlayerDead, PlayerHit, SwordAttack, SwordUpgrade,OpenInventory}//늘어날 예정..

    void Awake()
    {
        instance = this;
        Init();
        PlayBgm();
    }

    void PlayBgm()
    {
        bgmPlayer.Play();
    }

    void Init()
    {   
        //배경음 플레이어 초기화 
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent=transform;
        bgmPlayer=bgmObject.AddComponent<AudioSource>();//오디오소스생성
        bgmPlayer.playOnAwake=false;
        bgmPlayer.loop=true;
        bgmPlayer.volume=bgmVolume;
        bgmPlayer.clip=bgmClip;

        //효과음 플레이어 초기화
        GameObject sfxObject=new GameObject("SFXPlayer");
        sfxObject.transform.parent=transform;
        sfxPlayers = new AudioSource[channels];

        for(int index=0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index]=sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake=false;
            sfxPlayers[index].volume=sfxVolume;
        }
    }
    public void PlaySfx(Sfx sfx)
    {
        for(int index=0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index+channelIndex)%sfxPlayers.Length;
            
            if(sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex =0;
            /*if(sfx ==Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0,2);
            }
            */
            channelIndex=loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx+ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
        
    }
    
    
}
