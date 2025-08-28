using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

[Serializable]
public class Story
{
    public string storyName;
    public StoryState storyState;
    public string triggerStage;
    public string[] nextStoryName;
    public Action action = null;

    public Story(string storyName, StoryState storyState, string triggerStage = null, string[] nextStoryName = null, Action action = null)
    {
        this.storyName = storyName;
        this.storyState = storyState;
        this.triggerStage = triggerStage;
        this.nextStoryName = nextStoryName;
        this.action = action;
    }
}
