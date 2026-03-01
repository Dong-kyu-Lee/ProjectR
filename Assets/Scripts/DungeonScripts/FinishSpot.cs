using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FinishSpot : MonoBehaviour
{ 
    public bool isWaveEnd = false;
    private bool isPlayerTriggered = false;
    private string message = "'<color=yellow>E</color>'키를 눌러 다음 던전으로 이동";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 오브젝트에 2개의 트리거 콜라이더가 존재하여 FinishSpot과 닿았을 때, 해당 함수가 두 번 실행되는 것을 방지하기 위한 코드이다.
        if (collision.CompareTag("Player") && !isPlayerTriggered)
        {
            if (isWaveEnd)
            {
                InGameUIManager.Instance.ShowWarpUI(message,
                    () => {
                        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LobbyScene")
                            GameManager.Instance.MoveScene(SceneType.Normal, "DungeonGenerate");
                        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "UpgradeTestScene")
                            GameManager.Instance.MoveScene(SceneType.Normal, "DungeonGenerate");
                        else
                            DungeonFlowManager.Instance.GetCurrentStage().LoadNextDungeon();
                        gameObject.SetActive(false);
                    });
                isPlayerTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InGameUIManager.Instance.HideWarpUI();
            isPlayerTriggered = false;
        }
    }
}
