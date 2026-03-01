using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 던전의 방과 방 사이를 연결하는 워프를 나타내는 클래스
public class Warp : MonoBehaviour
{
    private Vector3 warpPoint;
    private bool isPlayerTriggered = false;
    private string message = "'<color=yellow>E</color>'키를 눌러 다음 방으로 이동";

    void Start()
    {
        
    }

    // 워프 될 곳의 위치를 설정하는 함수
    public void SetWarpPosition(Vector3 playerWarpPosition)
    {
        warpPoint = playerWarpPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPlayerTriggered)
        {
            isPlayerTriggered = true;
            // 플레이어가 워프에 닿았을 때, 플레이어를 워프 위치로 이동
            InGameUIManager.Instance.ShowWarpUI(message,
                () => { 
                    GameManager.Instance.CurrentPlayer.transform.position = warpPoint;
                    gameObject.SetActive(false);
                });
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
