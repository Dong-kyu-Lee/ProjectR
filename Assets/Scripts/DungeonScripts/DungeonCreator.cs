using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth;
    public int dungeonLength;

    [Header("Whole Random Only")]
    public int roomWidthMin;
    public int roomLengthMin;
    public int maxIteration;
    public int corridorWidth;

    void Start()
    {

        // CreateAllRandomDungeon();
    }

    private void CreateFixedRoomDungeon()
    {
        FixedRoomDungeonGenerator dungeonGenerator = new FixedRoomDungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = dungeonGenerator.SelectRooms();
    }

    private void CreateAllRandomDungeon()
    {
        AllRandomDungeonGenerator dungeonGenerator = new AllRandomDungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = dungeonGenerator.CalculateRooms(maxIteration, roomWidthMin, roomLengthMin);
    }
}
