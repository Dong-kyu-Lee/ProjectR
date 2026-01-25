using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;

    public void PauseTimeline()
    {
        director.Pause();
    }

    public void ResumeTimeline()
    {
        director.Play();
    }
}
