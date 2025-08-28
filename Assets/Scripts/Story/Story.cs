using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Story", menuName = "Story/Story")]
public class Story : ScriptableObject
{
    public string storyName;            // 스토리 이름
    public StoryState storyState;       // 스토리 상태
    public string sceneToLoad;          // 스토리 진행에 사용할 씬 이름
    public string[] nextStoryName;      // 다음 스토리 이름들
    public string description = null;   // 간단한 스토리 설명

    public Story(Story asset)
    {
        this.storyName = asset.storyName;
        this.storyState = asset.storyState;
        this.sceneToLoad = asset.sceneToLoad;
        this.nextStoryName = asset.nextStoryName;
        this.description = asset.description;
    }
}
