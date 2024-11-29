using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomContainer : MonoBehaviour
{
    public List<GameObject> rooms;

    void Start()
    {
        
    }

    // 열려야 할 통로에 대응할 수 있는 방들을 리스트로 가져오는 함수
    public List<GameObject> GetRooms(bool[] openNeededGate)
    {
        List<GameObject> listToReturn = new List<GameObject>();

        foreach (var room in rooms)
        {
            Room currentRoom = room.GetComponent<Room>();

            bool[] currentGateOfRoom =
                { currentRoom.isUpOpenable, currentRoom.isRightOpenable,
                currentRoom.isDownOpenable, currentRoom.isLeftOpenable };

            // 열려야 할 통로를 해당 방이 지원해주지 않는다면 해당 방을 건너뜀
            if ((openNeededGate[0] && !currentGateOfRoom[0]) ||
                (openNeededGate[1] && !currentGateOfRoom[1]) ||
                (openNeededGate[2] && !currentGateOfRoom[2]) ||
                (openNeededGate[3] && !currentGateOfRoom[3])) continue;

            listToReturn.Add(room);
        }

        return listToReturn;
    }
}
