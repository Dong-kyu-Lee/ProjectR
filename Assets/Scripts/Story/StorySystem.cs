using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StorySystem : MonoBehaviour
{
    public static StorySystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StorySystem>();
                // 이 클래스는 인스펙터의 stories[] 데이터에 의존하므로 자동 생성하지 않는다.
                // 없으면 씬 구성 문제이므로 에러로 즉시 드러낸다.
                if (instance == null)
                    Debug.LogError("StorySystem이 씬에 존재하지 않습니다. StartScene에 배치되어야 합니다.");
            }
            return instance;
        }
    }
    private static StorySystem instance;

    // 인스펙터에서 할당하는 원본 스토리 에셋들. 런타임 상태의 "초기값 원본"으로도 사용된다.
    public Story[] stories;

    // StoryID : 런타임 스토리 상태(원본을 복제한 인스턴스)
    private Dictionary<StoryID, Story> storyContainer = new Dictionary<StoryID, Story>();
    // 일회성 스토리(영속) 관리 위임 대상
    private SingleUseStoryManager singleUseStories;

    private string previousDungeonScene; // 스토리 씬 이동 전 던전 씬 이름
    private SceneType previousSceneType;  // 스토리 씬 이동 전 씬 타입 (복귀 시 사용)
    private string currentStoryScene;     // 현재 진행중인 스토리 씬 이름

    void Awake()
    {
        Init();

        // 일회성 스토리 로드/기본값 처리를 전담 매니저에 위임
        singleUseStories = new SingleUseStoryManager();

        // 스토리 등록: 원본 에셋을 통째로 복제해 런타임 상태를 원본과 분리한다.
        // (Instantiate 사용 → Story에 필드가 추가돼도 이 코드를 수정할 필요가 없다.)
        foreach (var source in stories)
        {
            if (source == null)
            {
                Debug.LogError("Null story in stories[].");
                continue;
            }
            if (storyContainer.ContainsKey(source.storyID))
            {
                Debug.LogError($"Duplicate story ID found: {source.storyID}");
                continue;
            }
            storyContainer.Add(source.storyID, Instantiate(source));
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
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 새로운 스토리 시작 처리
    // 플레이어의 상호작용에 의해서 호출됨(ex. NPC 대화, 보스 처치)
    public void StartStory(StoryID storyID)
    {
        // 스토리가 딕셔너리에 존재하는지 확인
        if (!storyContainer.ContainsKey(storyID))
        {
            Debug.LogError($"Story '{storyID}' does not exist.");
            return;
        }
        // 현재 스토리가 Available 상태인지 확인 (상태 머신 + nextStoryID 체인이 진행 순서를 통제)
        if (storyContainer[storyID].storyState != StoryState.Available)
        {
            Debug.LogError($"Story '{storyID}' is not available to start.");
            return;
        }
        // 스토리 상태를 In_Progress로 변경
        storyContainer[storyID].storyState = StoryState.In_Progress;

        // 스토리 씬으로 떠나기 전에 복귀에 필요한 현재 씬 정보를 저장
        previousDungeonScene = SceneManager.GetActiveScene().name;
        previousSceneType = GameManager.Instance.CurrentSceneType;
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
        // 다음 스토리가 있다면 상태를 Available로 변경 (선행 스토리 -> 후행 스토리 해금)
        foreach (var nextStory in storyContainer[storyID].nextStoryID)
        {
            if (storyContainer.ContainsKey(nextStory) && storyContainer[nextStory].storyState == StoryState.Locked)
            {
                storyContainer[nextStory].storyState = StoryState.Available;
            }
        }
        // 떠나온 던전 씬으로 복귀. 나갈 때와 동일하게 GameManager를 경유하여
        // 씬 타입별 복구(UI 생성/활성화 등)를 시스템이 일관되게 처리하도록 한다.
        GameManager.Instance.MoveScene(previousSceneType, previousDungeonScene);
    }

    // 플레이어가 죽으면 스토리 진행을 "작성된 초기 상태"로 되돌린다.
    // (재생 유지 방식: 클리어한 스토리도 초기 상태로 복원되어 다음 판에서 컷씬이 다시 재생됨)
    // 원본 stories[]는 런타임에 변형되지 않으므로 초기값의 신뢰 가능한 기준이 된다.
    public void ResetStory()
    {
        foreach (var source in stories)
        {
            if (source != null && storyContainer.TryGetValue(source.storyID, out var story))
            {
                story.storyState = source.storyState;
            }
        }
    }

    // 일회성 스토리 완료 처리 (전담 매니저에 위임)
    public void CompleteSingleUseStory(string storyName)
    {
        singleUseStories.Complete(storyName);
    }

    // 일회성 스토리 완료 여부 조회 (전담 매니저에 위임)
    public bool IsSingleUseStoryCompleted(string storyName)
    {
        return singleUseStories.IsCompleted(storyName);
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
