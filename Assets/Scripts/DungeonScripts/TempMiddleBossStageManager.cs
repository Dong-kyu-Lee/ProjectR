using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMiddleBossStageManager : MonoBehaviour
{
    [SerializeField]
    GameObject finishSpot;
    [SerializeField]
    GameObject playerSpawnPosition;
    [SerializeField]
    GameObject middleBoss;

    bool isMiddleBossDead = false;

    // 플레이어 스폰 위치에서 활성화
    // 잠시 대기 후 중간보스 활성화
    void Start()
    {
        if (finishSpot == null || playerSpawnPosition == null || middleBoss == null)
        {
            Debug.LogError("One or more required GameObjects are not assigned in the inspector.");
            return;
        }
        // 컷씬이 수행되었다면 플레이어 배치, 중간보스 비활성화, 피니시 스팟 활성화
        if(StorySystem.Instance.GetStoryState(StoryID.Temp_Middle_Moss) == StoryState.Completed)
        {
            GameManager.Instance.PlacePlayerObject(playerSpawnPosition.transform.position);
            GameManager.Instance.CurrentPlayer.SetActive(true);
            middleBoss.SetActive(false);
            finishSpot.SetActive(true);
            finishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
            return;
        }
        // 컷씬이 수행되지 않았다면 아래 코드 수행
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition.transform.position);
        GameManager.Instance.CurrentPlayer.SetActive(true);
        finishSpot.SetActive(false);
        StartCoroutine(MiddleBossActivateCoroutine());
    }

    void Update()
    {
        if(middleBoss.GetComponent<EnemyAIController>().isDead && !isMiddleBossDead)
        {
            isMiddleBossDead = true;
            OnMiddleBossStageEnd();
            // 중간보스 처치 후 컷씬 재생
            StorySystem.Instance.StartStory(StoryID.Temp_Middle_Moss);
        }
    }

    public void OnMiddleBossStageEnd()
    {
        middleBoss.SetActive(false);
        finishSpot.SetActive(true);
        finishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
    }

    IEnumerator MiddleBossActivateCoroutine()
    {
        yield return new WaitForSeconds(2f); // 2초 대기
        middleBoss.SetActive(true);
    }
}
