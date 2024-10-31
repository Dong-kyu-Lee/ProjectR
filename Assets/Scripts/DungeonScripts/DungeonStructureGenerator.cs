using System.Collections.Generic;
using UnityEngine;

public class DungeonStructureGenerator
{
    private int dungeonRow;
    private int dungeonColumn;
    private bool[] visited; // 각 방 위치를 방문했는지에 대한 bool값

    public DungeonStructureGenerator(int dungeonRow, int dungeonColumn)
    {
        this.dungeonRow = dungeonRow;
        this.dungeonColumn = dungeonColumn;
        visited = new bool[dungeonColumn*dungeonRow];
        // 0 1 2 3
        // 4 5 6 7
        // 8 9 10 11
        // 인덱스 값 / dungeonColumn = RoomColumn
        // 인덱스 값 % dungeonColumn = RoomRow
    }

    public void CreateDungeonStructure()
    {
        RoomNode root = new RoomNode(new Vector2Int(0, 0));

    }
}