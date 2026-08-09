using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject conversationUI;
    [SerializeField] NpcDialogue npcDialogue;
    [SerializeField] PrologueManager prologue;

    [Header("스킵 설정")]
    [SerializeField] GameObject skipButton;              // 우상단 스킵 버튼
    [SerializeField] PrologueSound prologueSound;        // 건너뛴 구간의 사운드 보정용
    [Tooltip("스킵 시 이동할 타임라인 시간(초). 음수면 타임라인 끝으로 이동")]
    [SerializeField] double skipTargetTime = -1;

    private bool isSkipping;
    private bool isEnded;

    private void Start()
    {
        if(prologue == null) prologue = FindObjectOfType<PrologueManager>();

        // 끝 지점의 시그널이 누락되더라도 프롤로그가 반드시 종료되도록 보장
        if (director != null) director.stopped += OnDirectorStopped;
    }

    private void OnDestroy()
    {
        if (director != null) director.stopped -= OnDirectorStopped;
    }

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

    // 스킵 버튼(Button.onClick)에 연결
    public void SkipPrologue()
    {
        if (isSkipping || isEnded) return;   // 연타 방지
        isSkipping = true;

        Debug.Log("Skip Prologue from TimelineManager");

        if (skipButton != null) skipButton.SetActive(false);

        // 1) 진행 중인 대화를 강제 종료하고 대화 UI를 정리
        npcDialogue.StopDialogue();
        conversationUI.SetActive(false);

        // 2) 타임라인 시간을 점프하면 건너뛴 구간의 Signal이 발생하지 않으므로
        //    엔딩 시점에 성립해 있어야 하는 상태를 직접 맞춰준다
        if (prologueSound != null) prologueSound.StopBGM();

        // 3) 목표 지점으로 이동 후 남은 연출을 재생
        double target = (skipTargetTime >= 0)
            ? System.Math.Min(skipTargetTime, director.duration)
            : director.duration - 0.01;

        director.time = target;
        director.Evaluate();    // 해당 시점의 오브젝트 상태를 즉시 반영
        director.Play();        // 대화 중에는 Pause 상태이므로 반드시 다시 Play
    }

    private void OnDirectorStopped(PlayableDirector _) => EndPrologue();

    public void EndPrologue()
    {
        if (isEnded) return;    // 시그널과 stopped 이벤트의 중복 호출 방지
        isEnded = true;

        Debug.Log("End Prologue from TimelineManager");
        prologue.CompleteCutScene();
    }
}
