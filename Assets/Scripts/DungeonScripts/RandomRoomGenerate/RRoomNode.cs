using UnityEngine;

public class RRoomNode
{
    private Vector2Int bottomLeft;
    private Vector2Int topRight;
    private Vector2 center; // 방의 정중앙

    public Vector2Int BottomLeft { get => bottomLeft; }
    public Vector2Int TopRight { get => topRight; }
    public Vector2 Center { get => center; }

    public RRoomNode(Vector2Int bottomLeft, Vector2Int topRight)
    {
        this.bottomLeft = bottomLeft;
        this.topRight = topRight;
        this.center = new Vector2((bottomLeft.x + topRight.x) / 2, (bottomLeft.y + topRight.y) / 2);
    }
}