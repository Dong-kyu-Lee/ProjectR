using System;
using System.Collections.Generic;
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
        // TODO: 방들을 잇는 그래프 생성
        DungeonStructureGenerator dungeonStructureGenerator
             = new DungeonStructureGenerator(dungeonRow, dungeonColumn);
        var roomNodes = dungeonStructureGenerator.CreateDungeonStructure();

        // TODO: 각 방 랜덤 선택

        // TODO: 조각의 통로와 벽, 위치 결정 후 리턴

        List<RoomNode> listToReturn = new List<RoomNode>();
        return listToReturn;
    }
}