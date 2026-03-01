using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCutSceneStory : MonoBehaviour
{
    public StoryID storyIDToComplete;

    public void ReceiveSignal()
    {
        StorySystem.Instance.CompleteStory(storyIDToComplete);
    }
}
