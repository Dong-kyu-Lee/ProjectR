using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonRow;
    public int dungeonColumn;
    public GameObject[] roomPrefabs;

    void Start()
    {
        CreateFixedRoomDungeon();
    }
    
    // 선택된 던전 조각들을 Instantiate하는 함수
    private void CreateFixedRoomDungeon()
    {
        FixedRoomDungeonGenerator dungeonGenerator = new FixedRoomDungeonGenerator(dungeonRow, dungeonColumn);
        var listOfRooms = dungeonGenerator.SelectRooms();

        // 조건에 맞는 방들을 랜덤으로 선택해 생성
        foreach( var room in listOfRooms )
        {
            
        }
    }
}
