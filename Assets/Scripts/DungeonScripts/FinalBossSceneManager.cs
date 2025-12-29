using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject finishSpot;
    [SerializeField]
    GameObject playerSpawnPosition;
    [SerializeField]
    GameObject finalBoss;
    [SerializeField]
    CinemachineVirtualCamera finalBossCam;
    [SerializeField]
    BossHealthUI bossHealthUI;

    bool isFinalBossDead = false;

    void Start()
    {
        if (finishSpot == null || playerSpawnPosition == null || finalBoss == null)
        {
            Debug.LogError("One or more required GameObjects are not assigned in the inspector.");
            return;
        }
        // 중간보스 처치 컷씬이 수행되었다면 플레이어 배치, 중간보스 비활성화, 피니시 스팟 활성화
        if (StorySystem.Instance.GetStoryState(StoryID.Temp_Final_Boss) == StoryState.Completed)
        {
            GameManager.Instance.SetActiveInGameUI(); // 인게임 UI 활성화
            GameManager.Instance.PlacePlayerObject(playerSpawnPosition.transform.position);
            GameManager.Instance.CurrentPlayer.SetActive(true);
            finalBoss.SetActive(false);
            finishSpot.SetActive(true);
            finishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
            return;
        }
        // 컷씬이 수행되지 않았다면 아래 코드 수행 (최종보스전 시작)
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition.transform.position);
        GameManager.Instance.CurrentPlayer.SetActive(true);
        finalBossCam.Follow = GameManager.Instance.CurrentPlayer.transform;
        finishSpot.SetActive(false);
        StartCoroutine(FinalBossActivateCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (finalBoss.GetComponent<EnemyAIController>().isDead && !isFinalBossDead)
        {
            isFinalBossDead = true;
            StartCoroutine(FinalBossDeadCoroutine());
        }
    }

    // 최종보스 활성화 대기 코루틴
    IEnumerator FinalBossActivateCoroutine()
    {
        yield return new WaitForSeconds(2f); // 2초 대기
        finalBoss.SetActive(true);

        if (bossHealthUI != null)
        {
            bossHealthUI.gameObject.SetActive(true);
            bossHealthUI.SetBoss(finalBoss.GetComponent<EnemyStatus>());
        }
    }

    IEnumerator FinalBossDeadCoroutine()
    {
        yield return new WaitForSeconds(2.5f); // 2.5초 대기
        finalBoss.SetActive(false);
        // 최종보스 처치 후 컷씬 재생
        StorySystem.Instance.SetStoryState(StoryID.Temp_Final_Boss, StoryState.Available);
        StorySystem.Instance.StartStory(StoryID.Temp_Final_Boss);
    }
}
