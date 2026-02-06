using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockGhost;
    public GameObject[] unlockGhost;
    public GameObject uiNotice;

    //유령 네이밍 추후 변경
    enum Achive { Unlock_2, Unlock_3, Unlock_4, Unlock_5 }
    Achive[] achives;
    WaitForSecondsRealtime wait;

    void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);

        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0); //해금 전 : 0
        }
    }
    void Start()
    {
        UnlockGhost();
    }
    void UnlockGhost()
    {
        for (int i = 0; i < lockGhost.Length; i++)
        {
            string achiveName = achives[i].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1; //해금 조건 isUnlock 0 -> 1
            lockGhost[i].SetActive(!isUnlock);
            unlockGhost[i].SetActive(isUnlock);
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        foreach (Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }
    void CheckAchive(Achive achive)
    {
        bool isAchive = false;
        //업적 목록
        //경험치 휙득 = 업적이므로 제조 완료 -> 메인 화면 : 손님 떠난 후 경험치 상승시키기
        //팝업 종료 후 손님 들어오도록 해야됨 -> 이건 손님 타이머에서 관리 ㄱㄱ

        switch (achive)
        {   //테스트용으로 2는 잠금, 3은 해금
            case Achive.Unlock_2:
                isAchive = false;
                break;
            case Achive.Unlock_3:
                isAchive = true;
                break;
            /*
            case Achive.Unlock_4:
                isAchive = true;
                break;
            case Achive.Unlock_5:
                isAchive = true;
                break;
            */
        }
        //업적 달성 시 PlayerPref를 0->1 로 변화
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);

            for (int i = 0; i < uiNotice.transform.childCount; i++)
            {
                bool isActive = i == (int)achive;
                uiNotice.transform.GetChild(i).gameObject.SetActive(isActive);
            }

            StartCoroutine(NoticeRoutine());

        }
    }
    IEnumerator NoticeRoutine()
    {
        //효과음 추후 추가
        uiNotice.SetActive(true);
        yield return wait;
        uiNotice.SetActive(false);
    }
}

