using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonRow;
    public int dungeonColumn;

    void Start()
    {

    }
    
    // 선택된 던전 조각들을 Instantiate하는 함수
    private void CreateFixedRoomDungeon()
    {
        FixedRoomDungeonGenerator dungeonGenerator = new FixedRoomDungeonGenerator(dungeonRow, dungeonColumn);
        var listOfRooms = dungeonGenerator.SelectRooms();
    }
}
