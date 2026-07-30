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
        // 최초 실행에는 파일이 없는 것이 정상. 완료 기록이 생기는 시점에 저장된다.
        if (!File.Exists(path)) return new SingleUseStory();

        var data = JsonUtility.FromJson<SingleUseStory>(File.ReadAllText(path));
        if (data == null)
        {
            Debug.LogError($"Failed to parse single-use story data at path: {path}");
            return new SingleUseStory();
        }
        if (data.stories == null) data.stories = new List<StoryData>();
        return data;
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
