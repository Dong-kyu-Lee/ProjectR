using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum StoryID
{
    Temp_Middle_Boss, Temp_Final_Boss, Test,
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
}
