using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Dictionary<StoryID, Story> storyContainer = new Dictionary<StoryID, Story>();
    // 일회성 스토리 데이터
    private SingleUseStory singleUseStory;

    private string previousDungeonScene; // 스토리 씬 이동 전 던전 씬 이름
    private string currentStoryScene; // 현재 진행중인 스토리 씬 이름

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
            if (!storyContainer.ContainsKey(stories[i].storyID))
            {
                Story story = ScriptableObject.CreateInstance<Story>();
                story.storyID = stories[i].storyID;
                story.storyState = stories[i].storyState;
                story.sceneToLoad = stories[i].sceneToLoad;
                story.nextStoryID = stories[i].nextStoryID;
                story.description = stories[i].description;

                storyContainer.Add(stories[i].storyID, story);
            }
            else
            {
                Debug.LogError($"Duplicate story name found: {stories[i].storyID}");
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
    public void StartStory(StoryID storyID)
    {
        // 스토리가 딕셔너리에 존재하는지 확인
        if (!storyContainer.ContainsKey(storyID))
        {
            Debug.LogError($"Story '{storyID}' does not exist.");
            return;
        }
        // 현재 스토리가 Available 상태인지 확인
        if (storyContainer[storyID].storyState != StoryState.Available)
        {
            Debug.LogError($"Story '{storyID}' is not available to start.");
            return;
        }
        // 스토리 상태를 In_Progress로 변경
        storyContainer[storyID].storyState = StoryState.In_Progress;
        // 씬 로드
        previousDungeonScene = SceneManager.GetActiveScene().name;
        currentStoryScene = storyContainer[storyID].sceneToLoad;
        GameManager.Instance.MoveScene(SceneType.StoryScene, currentStoryScene);
    }

    // 스토리 완료 처리
    public void CompleteStory(StoryID storyID)
    {
        // 스토리가 딕셔너리에 존재하는지 확인
        if (!storyContainer.ContainsKey(storyID))
        {
            Debug.LogError($"Story '{storyID}' does not exist.");
            return;
        }
        // 스토리 상태를 Completed로 변경
        storyContainer[storyID].storyState = StoryState.Completed;
        // 다음 스토리가 있다면 상태를 Available로 변경
        foreach (var nextStory in storyContainer[storyID].nextStoryID)
        {
            if (storyContainer.ContainsKey(nextStory) && storyContainer[nextStory].storyState == StoryState.Locked)
            {
                storyContainer[nextStory].storyState = StoryState.Available;
            }
        }
        // 이전 던전 씬으로 복귀
        SceneManager.LoadScene(previousDungeonScene);
    }

    // 플레이어가 죽으면 스토리 진행 초기화. (일회성 스토리는 초기화하지 않음)
    // 클리어한 스토리는 유지
    public void ResetStory()
    {
        foreach (var story in storyContainer.Values)
        {
            story.storyState = StoryState.Locked;
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

    public StoryState GetStoryState(StoryID storyID)
    {
        if (storyContainer.ContainsKey(storyID))
        {
            return storyContainer[storyID].storyState;
        }
        Debug.LogError($"Story '{storyID}' does not exist.");
        return StoryState.Locked; // 기본값 반환
    }

    public void SetStoryState(StoryID storyID, StoryState state)
    {
        if (storyContainer.ContainsKey(storyID))
        {
            storyContainer[storyID].storyState = state;
        }
        else
        {
            Debug.LogError($"Story '{storyID}' does not exist.");
        }
    }
}
