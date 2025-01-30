using UnityEngine;

public class RRoomNode
{
    private Vector2Int bottomLeft;
    private Vector2Int topRight;
    private Vector2 central; // 방의 정 중앙
    private NextRoomDirection nextRoomDirection; // 플레이어가 다음으로 이동할 방으로의 이동 방향

    public Vector2Int BottomLeft { get => bottomLeft; }
    public Vector2Int TopRight { get => topRight; }
    public NextRoomDirection NextRoomDirection { get => nextRoomDirection; }
    public Vector2 Central { get => central; }

    public RRoomNode(Vector2Int bottomLeft, Vector2Int topRight)
    {
        this.bottomLeft = bottomLeft;
        this.topRight = topRight;
        this.central = new Vector2((bottomLeft.x + topRight.x) / 2, (bottomLeft.y + topRight.y) / 2);
    }
}