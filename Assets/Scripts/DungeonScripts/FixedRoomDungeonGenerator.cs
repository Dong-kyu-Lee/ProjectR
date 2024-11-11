using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class FixedRoomDungeonGenerator
{
    private int dungeonWidth;
    private int dungeonLength;
    private int dungeonColumn; // 열
    private int dungeonRow; // 행
    

    public FixedRoomDungeonGenerator(int dungeonRow, int dungeonColumn)
    {
        this.dungeonRow = dungeonRow;
        this.dungeonColumn = dungeonColumn;
    }

    // 던전 조각들을 알고리즘에 따라 선택하는 함수
    public List<RoomNode> SelectRooms()
    {
        // 방들을 잇는 그래프 생성
        DungeonStructureGenerator dungeonStructureGenerator
             = new DungeonStructureGenerator(dungeonRow, dungeonColumn);

        // 조각의 통로와 벽, 위치 결정 후 리턴
        var roomNodes = dungeonStructureGenerator.CreateDungeonStructure();
        List<RoomNode> listToReturn = new List<RoomNode>();
        for(int i = 0; i < roomNodes.Count; ++i)
        {
            for(int j = 0;j <  roomNodes[i].Count; ++j)
            {
                listToReturn.Add(roomNodes[i][j]);
            }
        }
        return listToReturn;
    }
}