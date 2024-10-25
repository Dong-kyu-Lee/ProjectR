using System;
using UnityEngine;

public class RoomNode : Node
{
    public RoomNode(
        Vector2Int bottomLeftAreaCorner,
        Vector2Int topRightAreaCorner,
        Node parentNode,
        int index) : base(parentNode)
    {
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(TopRightAreaCorner.x, BottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(BottomLeftAreaCorner.x, TopRightAreaCorner.y);
        this.TreeLayerIndex = index;
    }

    public int Width { get => (int)(TopRightAreaCorner.x - TopLeftAreaCorner.x); }
    public int Length { get => (int)(TopLeftAreaCorner.y - BottomLeftAreaCorner.y); }
}