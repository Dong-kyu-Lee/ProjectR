using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StoryState
{
    Locked, Available, In_Progress, Completed,
}

public class StorySystem : MonoBehaviour
{
    public static StorySystem Instance {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StorySystem>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("StorySystem");
                    instance = obj.AddComponent<StorySystem>();
                }
            }
            return instance;
        }
    }
    private static StorySystem instance;

    public TextAsset singleUseStoryData;
    public Story[] stories;

    // string : 스토리 이름, Story : 스토리 오브젝트
    private Dictionary<string, Story> storyContainer = new Dictionary<string, Story>();

    void Start()
    {
        Init();

        // JSON 데이터 파싱 (SingleUseStory)
        SingleUseStory storyData = JsonUtility.FromJson<SingleUseStory>(singleUseStoryData.text);
        foreach (var data in storyData.stories)
        {
            Debug.Log($"Story Name: {data.name}, Value: {data.value}");
        }

        // 스토리 데이터 추가
        for (int i = 0; i < stories.Length; i++)
        {
            if (!storyContainer.ContainsKey(stories[i].storyName))
            {
                storyContainer.Add(stories[i].storyName, new Story(stories[i]));
            }
            else
            {
                Debug.LogError($"Duplicate story name found: {stories[i].storyName}");
            }
        }
    }

    // 싱글톤 초기화
    private void Init()
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

    // 새로운 스토리 시작 처리
    // 플레이어의 상호작용에 의해서 호출됨(ex. NPC 대화, 씬 이동)
    public void StartStory(string storyName)
    {
        // 스토리가 딕셔너리에 존재하는지 확인
        if (!storyContainer.ContainsKey(storyName))
        {
            Debug.LogError($"Story '{storyName}' does not exist.");
            return;
        }
        // 스토리의 선행 스토리가 완료되었는지 확인

        // 스토리 상태를 In_Progress로 변경
    }

    // 스토리 완료 처리
    public void CompleteStory(string storyName)
    {
        // 스토리가 딕셔너리에 존재하는지 확인
        // 스토리 상태를 Completed로 변경
        // 다음 스토리가 있다면 상태를 Available로 변경
    }

    // 플레이어가 죽으면 스토리 진행 초기화. (일회성 스토리는 초기화하지 않음)
    public void ResetStory()
    {

    }
}
