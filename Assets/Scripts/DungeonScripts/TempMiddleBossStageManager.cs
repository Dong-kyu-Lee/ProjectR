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
