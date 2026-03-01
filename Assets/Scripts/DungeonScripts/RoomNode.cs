using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum OpenedGate
{
    Up, Right, Down, Left
}

public class RoomNode
{
    private Vector2Int roomPosition;
    private bool[] openNeededGate = { false, false, false, false }; // 위, 오른쪽, 아래, 왼쪽 (시계방향)

    public bool[] OpenNeededGate { get => openNeededGate; }
    public Vector2Int RoomPosition { get => roomPosition; set => roomPosition = value; }

    public RoomNode(Vector2Int roomPosition)
    {
        this.roomPosition = roomPosition;
    }

    // 현재 노드의 위치와 pos위치를 비교해 4개의 통로 중 어떤 곳을 열어야 할 지 결정하는 함수
    public void CalculateWhichGateNeedOpen(Vector2Int pos)
    {
        if (roomPosition + new Vector2Int(0, 1) == pos) openNeededGate[0] = true;
        else if (roomPosition + new Vector2Int(1, 0) == pos) openNeededGate[1] = true;
        else if (roomPosition + new Vector2Int(0, -1) == pos) openNeededGate[2] = true;
        else if (roomPosition + new Vector2Int(-1, 0) == pos) openNeededGate[3] = true;
    }
}
