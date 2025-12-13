using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{

    void Start()
    {
        
    }

    void LateUpdate()
    {
        Vector3 playerPosition = GameManager.Instance.CurrentPlayer.transform.position;
        var currentStage = DungeonFlowManager.Instance.GetCurrentStage();
        if (currentStage == null) return;
        var roomInfos = currentStage.roomList;
        for (int i = 0; i < roomInfos.Count; ++i)
        {
            if(roomInfos[i] == null)
            {
                Debug.LogError("MinimapCamera: roomInfos[" + i + "] is null");
                continue;
            }
            if (roomInfos[i].GetRoomState != RoomState.Default)
            {
                Vector3 roomPosition = roomInfos[i].transform.position;
                float distanceX = playerPosition.x - roomPosition.x;
                float distanceY = playerPosition.y - roomPosition.y;
                if (distanceX <= 40 && distanceX >= 0 && distanceY <= 40 && distanceY >= 0)
                {
                    // 플레이어가 해당 방에 위치해있다는 뜻
                    // 해당 방의 가운데에 미니맵 카메라를 위치시킴
                    transform.position = new Vector3(roomPosition.x + 19.5f, roomPosition.y + 19.5f, transform.position.z);
                    break;
                }
            }
        }
    }
}
