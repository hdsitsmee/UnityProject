using System;
using UnityEngine;

[Serializable]
public class Data
{
    public int level = 1;
    public int money = 2500;
}
public static class MainData
{
    private const string SAVE_KEY = "Json";

    public static Data Load()
    {
            // 첫 실행
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return new Data();
            // 저장된 문자열 가져오기
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
            //저장 키 Null인 경우
        if (string.IsNullOrEmpty(json))
            return new Data();
            // 기존 데이터 Load
        return JsonUtility.FromJson<Data>(json);
    }
    public static void Save (int money, int level)
    {
        Data data = new Data { money = money, level = level };
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }
}
