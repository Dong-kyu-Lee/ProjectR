using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataManager
{
    private const string singleUseStoryPath = "SingleUseStory.json";

    // 일회성 스토리 json 데이터 불러오기
    public static SingleUseStory LoadSingleUseStoryData()
    {
        string path = Path.Combine(Application.persistentDataPath, singleUseStoryPath);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"File not found at path: {path}");
            SaveSingleUseStoryData(new SingleUseStory()); // 기본 데이터 저장
        }
        if(!File.Exists(path))
        {
            Debug.LogError($"File still not found at path: {path}");
            return new SingleUseStory(); // 빈 데이터 반환
        }
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SingleUseStory>(json);
    }

    // 일회성 스토리 json 데이터 저장하기
    public static void SaveSingleUseStoryData(SingleUseStory data)
    {
        string path = Path.Combine(Application.persistentDataPath, singleUseStoryPath);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"Data saved to path: {path}");
    }
}
