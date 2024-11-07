using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum OpenedWay
{
    Up, Right, Down, Left
}

public class RoomNode
{
    private Vector2Int roomPosition;
    private Vector2Int parentRoomPosition;
    private List<Vector2Int> childrenRoomPositions;
    // 위, 오른쪽, 아래, 왼쪽 (시계방향)
    private bool[] openNeededWay = { false, false, false, false };

    public Vector2Int RoomPosition { get => roomPosition; set => roomPosition = value; }
    public Vector2Int ParentRoomPosition { get => parentRoomPosition; set => parentRoomPosition = value; }

    public RoomNode(Vector2Int roomPosition)
    {
        this.roomPosition = roomPosition;
        childrenRoomPositions = new List<Vector2Int>();
    }

    public void AddChildrenRoomPosition(Vector2Int roomPosition)
    {
        childrenRoomPositions.Add(roomPosition);
    }

    // 현재 노드의 위치와 pos위치를 비교해 4개의 통로 중 어떤 곳과 연결되어 있는지 판단하는 함수
    private void CalculateWhichWayNeedOpen(Vector2Int pos)
    {
        if (roomPosition + new Vector2Int(0, 1) == pos) openNeededWay[0] = true;
        else if (roomPosition + new Vector2Int(1, 0) == pos) openNeededWay[1] = true;
        else if (roomPosition + new Vector2Int(0, -1) == pos) openNeededWay[2] = true;
        else if (roomPosition + new Vector2Int(-1, 0) == pos) openNeededWay[3] = true;
    }
}
