using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public Story[] stories; 

    // string : 스토리 이름, Story : 스토리 오브젝트
    private Dictionary<string, Story> storyContainer = new Dictionary<string, Story>();
    // 일회성 스토리 데이터
    private SingleUseStory singleUseStory;

    void Awake()
    {
        Init();

        // 일회성 스토리 데이터 로드
        singleUseStory = DataManager.LoadSingleUseStoryData();
        if(singleUseStory.stories.Count == 0)
        {
            // 기본 일회성 스토리 데이터 설정
            singleUseStory.stories.Add(new StoryData { name = "Prologue", value = false });
            singleUseStory.stories.Add(new StoryData { name = "In the bar", value = false });
            DataManager.SaveSingleUseStoryData(singleUseStory);
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
        // 현재 스토리가 Available 상태인지 확인
        if (storyContainer[storyName].storyState != StoryState.Available)
        {
            Debug.LogError($"Story '{storyName}' is not available to start.");
            return;
        }
        // 스토리 상태를 In_Progress로 변경
        storyContainer[storyName].storyState = StoryState.In_Progress;
        // 씬 로드 (필요시)
    }

    // 스토리 완료 처리
    public void CompleteStory(string storyName)
    {
        // 스토리가 딕셔너리에 존재하는지 확인
        if (!storyContainer.ContainsKey(storyName))
        {
            Debug.LogError($"Story '{storyName}' does not exist.");
            return;
        }
        // 스토리 상태를 Completed로 변경
        storyContainer[storyName].storyState = StoryState.Completed;
        // 다음 스토리가 있다면 상태를 Available로 변경
        foreach (var nextStory in storyContainer[storyName].nextStoryName)
        {
            if (storyContainer.ContainsKey(nextStory) && storyContainer[nextStory].storyState == StoryState.Locked)
            {
                storyContainer[nextStory].storyState = StoryState.Available;
            }
        }
    }

    // 플레이어가 죽으면 스토리 진행 초기화. (일회성 스토리는 초기화하지 않음)
    // 클리어한 스토리는 유지
    public void ResetStory()
    {
        foreach (var story in storyContainer.Values)
        {
            if (story.storyState == StoryState.In_Progress)
            {
                story.storyState = StoryState.Available;
            }
        }
    }

    // 일회성 스토리 완료 처리
    public void CompleteSingleUseStory(string storyName)
    {
        var storyData = singleUseStory.stories.Find(s => s.name == storyName);
        if (storyData != null)
        {
            storyData.value = true;
            Debug.Log($"Single-use story '{storyName}' marked as completed.");
        }
        else
        {
            Debug.LogError($"Single-use story '{storyName}' not found.");
        }
        // 변경된 데이터를 저장
        DataManager.SaveSingleUseStoryData(singleUseStory);
    }
}
