using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    private Vector2Int roomPosition;
    private Vector2Int parentRoomPosition;
    private List<Vector2Int> childrenRoomPositions;

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
}
