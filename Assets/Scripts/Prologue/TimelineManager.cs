using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject conversationUI;
    [SerializeField] NpcDialogue npcDialogue;

    public void PauseTimeline()
    {
        Debug.Log("Pausing Timeline");
        director.Pause();
        conversationUI.SetActive(true);
    }

    public void ResumeTimeline()
    {
        Debug.Log("Resuming Timeline");
        director.Play();
        conversationUI.SetActive(false);
    }

    public void StartDialogue()
    {
        Debug.Log("Start Dialogue from TimelineManager");
        conversationUI.SetActive(true);
        npcDialogue.RunDialogue();
    }

    public void EndPrologue()
    {
        Debug.Log("End Prologue from TimelineManager");
        GameManager.Instance.prologue.CompleteCutScene();
    }
}
