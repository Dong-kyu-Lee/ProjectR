using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    private Vector2Int roomPosition;
    private Vector2Int parentRoomPosition;
    private List<Vector2Int> childrenRoomPositions;

    public Vector2Int RoomPosition { get => roomPosition; }
    public Vector2Int ParentRoomPosition { get => parentRoomPosition; }

    public RoomNode(Vector2Int roomPosition, RoomNode parentNode = null)
    {
        this.roomPosition = roomPosition;
        childrenRoomPositions = new List<Vector2Int>();
        if (parentNode == null) parentNode = this;
        this.parentRoomPosition = parentNode.ParentRoomPosition;
    }

    public void AddChildrenRoomPosition(Vector2Int roomPosition)
    {
        childrenRoomPositions.Add(roomPosition);
    }
}
