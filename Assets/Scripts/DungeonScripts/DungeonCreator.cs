using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth;
    public int dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIteration;
    public int corridorWidth;

    void Start()
    {
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        DungeonGenerator dungeonGenerator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = dungeonGenerator.CalculateRooms(maxIteration, roomWidthMin, roomLengthMin);
    }
}
