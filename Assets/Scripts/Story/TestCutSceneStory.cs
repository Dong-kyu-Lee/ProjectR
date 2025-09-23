using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCutSceneStory : MonoBehaviour
{
    public void ReceiveSignal()
    {
        StorySystem.Instance.CompleteStory(StoryID.Temp_Middle_Moss);
    }
}
