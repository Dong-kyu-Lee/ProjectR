using Cinemachine;
using System.Collections;
using UnityEngine;

// 중간보스/최종보스처럼 "보스 처치 -> 스토리 컷씬 -> 복귀" 흐름을 공유하는 보스 씬 매니저의 공통 부모.
// 서로 다른 값(대상 StoryID, BGM 경로, 대기시간)만 자식이 제공하고, 흐름 자체는 이 클래스가 가진다.
public abstract class BossStorySceneManager : MonoBehaviour
{
    [SerializeField]
    protected GameObject finishSpot;
    [SerializeField]
    protected GameObject playerSpawnPosition;
    [SerializeField]
    protected GameObject boss;
    [SerializeField]
    protected CinemachineVirtualCamera bossCam;
    [SerializeField]
    protected BossHealthUI bossHealthUI;

    private bool isBossDead = false;

    // 자식이 채워 넣는 차이점
    protected abstract StoryID TargetStoryID { get; }
    protected abstract string BgmPath { get; }
    protected virtual float ActivateDelay => 2f;
    protected virtual float DeadDelay => 2f;

    protected virtual void Start()
    {
        if (finishSpot == null || playerSpawnPosition == null || boss == null)
        {
            Debug.LogError("One or more required GameObjects are not assigned in the inspector.");
            return;
        }

        PlacePlayer();
        SoundManager.Instance.Play(BgmPath, Sound.Bgm);

        // 이미 컷씬을 본 경우(Completed): 보스 스킵, 클리어 상태로 배치하고 피니시 스팟 활성화
        if (StorySystem.Instance.GetStoryState(TargetStoryID) == StoryState.Completed)
        {
            GameManager.Instance.SetActiveInGameUI(); // 인게임 UI 활성화
            OnBossStageEnd();
            return;
        }

        // 처음 진입: 보스전 시작
        bossCam.Follow = PlayerManager.Instance.CurrentPlayer.transform;
        finishSpot.SetActive(false);
        boss.SetActive(false);
        StartCoroutine(BossActivateCoroutine());
    }

    protected virtual void Update()
    {
        // 보스가 죽는 순간 단 한 번만 사망 처리 코루틴을 시작
        if (!isBossDead && boss.GetComponent<EnemyAIController>().isDead)
        {
            isBossDead = true;
            StartCoroutine(BossDeadCoroutine());
        }
    }

    private void PlacePlayer()
    {
        PlayerManager.Instance.PlacePlayerObject(playerSpawnPosition.transform.position);
        PlayerManager.Instance.CurrentPlayer.SetActive(true);
    }

    // 보스 처치 시 오브젝트 상태 변경 & 피니시 스팟 활성화
    protected void OnBossStageEnd()
    {
        boss.SetActive(false);
        finishSpot.SetActive(true);
        finishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
    }

    // 보스 활성화 대기 코루틴
    private IEnumerator BossActivateCoroutine()
    {
        yield return new WaitForSeconds(ActivateDelay);
        boss.SetActive(true);

        if (bossHealthUI != null)
        {
            bossHealthUI.gameObject.SetActive(true);
            // 보스 오브젝트에서 Status를 찾아 UI에 연결
            bossHealthUI.SetBoss(boss.GetComponent<EnemyStatus>());
        }
    }

    // 보스 처치 후 컷씬 재생 코루틴
    private IEnumerator BossDeadCoroutine()
    {
        yield return new WaitForSeconds(DeadDelay);
        OnBossStageEnd();
        // 상태 머신/nextStoryID 체인이 진행 순서를 통제한다. (강제 상태 변경 없이 시작 요청)
        StorySystem.Instance.StartStory(TargetStoryID);
    }
}
