using Cinemachine;
using System.Collections;
using UnityEngine;

// 중간보스/최종보스처럼 "보스 처치 -> 스토리 컷씬 -> 복귀" 흐름을 공유하는 보스 씬 매니저의 공통 부모.
// 재생할 스토리와 처치 여부는 현재 스테이지 노드(Stage.CurrentNode)가 결정하고, 자식은 대기시간 등 차이점만 제공한다.
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
    private Stage stage;

    // 자식이 채워 넣는 차이점
    // 스테이지 흐름 없이 보스 씬을 단독 실행(테스트)할 때 사용할 스토리. 평소에는 현재 노드의 storyID를 사용한다.
    protected abstract StoryID FallbackStoryID { get; }
    protected virtual float ActivateDelay => 2f;
    protected virtual float DeadDelay => 2f;

    private StageNode CurrentNode => stage != null ? stage.CurrentNode : null;
    private StoryID TargetStoryID => CurrentNode != null ? CurrentNode.storyID : FallbackStoryID;
    private bool ShouldPlayStory => CurrentNode == null || CurrentNode.playStory;

    // 보스를 이미 처치했는지 여부. 컷씬 복귀로 씬이 다시 로드된 경우 true
    private bool IsBossCleared
    {
        get
        {
            if (CurrentNode != null) return stage.IsCurrentNodeCleared;
            return StorySystem.Instance.GetStoryState(FallbackStoryID) == StoryState.Completed;
        }
    }

    protected virtual void Start()
    {
        if (finishSpot == null || playerSpawnPosition == null || boss == null)
        {
            Debug.LogError("One or more required GameObjects are not assigned in the inspector.");
            return;
        }

        stage = DungeonFlowManager.Instance.GetCurrentStage();
        PlacePlayer();

        // 이미 보스를 처치한 경우(컷씬 복귀): 보스 스킵, 클리어 상태로 배치하고 피니시 스팟 활성화
        if (IsBossCleared)
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
        // 컷씬 씬으로 이동했다가 돌아와도 보스가 다시 등장하지 않도록 처치 상태를 먼저 기록
        if (CurrentNode != null) stage.IsCurrentNodeCleared = true;
        // 스토리 상태는 BossState 진입 시 Available로 준비된다. (스테이지마다 컷씬 재생)
        if (ShouldPlayStory) StorySystem.Instance.StartStory(TargetStoryID);
    }
}
