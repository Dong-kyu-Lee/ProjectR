using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum StoryID
{
    Temp_Middle_Boss, Temp_Final_Boss, Test,
    // enum 값이 에셋에 int로 직렬화되므로 새 항목은 반드시 끝에 추가한다.
    Prologue, FirstLobbyEntry,
}

public enum StoryState
{
    Locked, Available, In_Progress, Completed,
}

[CreateAssetMenu(fileName = "New Story", menuName = "Story/Story")]
public class Story : ScriptableObject
{
    public StoryID storyID;            // 스토리 이름
    public StoryState storyState;       // 스토리 상태
    public string sceneToLoad;          // 스토리 진행에 사용할 씬 이름
    public StoryID[] nextStoryID;      // 다음 스토리 이름들
    public string description = null;   // 간단한 스토리 설명

    [Header("일회성 스토리")]
    // true면 완료 여부가 json에 영구 기록되고, ResetStory 대상에서 제외된다.
    public bool isSingleUse;
    // 진입 전 씬으로 복귀하지 않고 고정된 씬으로 나가는 스토리(ex. 프롤로그 -> 로비)
    public bool useFixedReturnScene;
    public SceneType returnSceneType = SceneType.LobbyScene;
    public string returnSceneName = "LobbyScene";
}
