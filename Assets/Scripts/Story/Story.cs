using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Story
{
    public string storyName;
    public StoryState storyState;
    public string triggerStage;
    public string nextStoryName;

    public void Start()
    {
        SceneManager.LoadScene(triggerStage);
    }
}
