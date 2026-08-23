using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq; // BuffManager의 딕셔너리 키를 리스트로 변환하기 위해 필요합니다.

public class FinishSpot : MonoBehaviour
{
    public bool isWaveEnd = false;
    private bool isPlayerTriggered = false;
    private string message = "'<color=yellow>E</color>'키를 눌러 다음 던전으로 이동";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 오브젝트에 2개의 트리거 콜라이더가 존재하여 FinishSpot과 닿았을 때, 해당 함수가 두 번 실행되는 것을 방지
        if (collision.CompareTag("Player") && !isPlayerTriggered)
        {
            if (isWaveEnd)
            {
                InGameUIManager.Instance.ShowWarpUI(message,
                    () => {
                        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                        // 로비 씬이나 연습장 씬에서 던전으로 넘어갈 때 각종 상태와 인벤토리를 초기화합니다.
                        if (currentScene == "LobbyScene" || currentScene == "UpgradeTestScene")
                        {
                            // 1. 투척 아이템 초기화
                            Inventory playerInventory = collision.GetComponentInChildren<Inventory>();
                            if (playerInventory != null)
                            {
                                playerInventory.ClearAllThrowableItems();
                            }

                            // 2. 체력 최대치로 회복
                            PlayerStatus playerStatus = collision.GetComponent<PlayerStatus>();
                            if (playerStatus != null)
                            {
                                playerStatus.Hp = playerStatus.MaxHp;
                                Debug.Log("[FinishSpot] 던전 진입 전 체력을 최대로 회복했습니다.");
                            }

                            // 3. 묻어있는 디버프 (및 버프) 싹 지우기
                            BuffManager buffManager = collision.GetComponent<BuffManager>();
                            if (buffManager != null)
                            {
                                // 딕셔너리의 키(BuffType) 리스트를 미리 복사해두고 지워야 컬렉션 변경 에러가 안 납니다.
                                var activeBuffTypes = buffManager.ActiveBuffDict.Keys.ToList();
                                foreach (var buffType in activeBuffTypes)
                                {
                                    buffManager.DeactivateBuff(buffType);
                                }
                                Debug.Log("[FinishSpot] 던전 진입 전 몸에 묻은 모든 버프/디버프를 제거했습니다.");
                            }

                            GameManager.Instance.MoveScene(SceneType.Normal, "DungeonGenerate");
                        }
                        else
                        {
                            DungeonFlowManager.Instance.GetCurrentStage().LoadNextDungeon();
                        }
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