using System;
using System.Collections.Generic;
using UnityEngine;

public class AllRandomDungeonGenerator
{
    List<RoomNode> allSpaceNodes = new List<RoomNode>();
    private int dungeonWidth;
    private int dungeonLength;

    public AllRandomDungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    // 던전의 각 방을 생성하는 함수
    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);
        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);

        List<Node> roomSpace = StructureHelper.TraverseGraphToExtractLeafs(bsp.RootNode);
        // 공간 안에 방을 생성할 클래스
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomWidthMin, roomLengthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomInGivenSpace(roomSpace);

        return new List<Node>(roomList);
    }
}