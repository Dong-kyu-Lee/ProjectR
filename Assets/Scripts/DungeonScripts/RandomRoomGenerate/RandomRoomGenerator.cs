using System.Collections.Generic;
using UnityEngine;

public enum NextRoomDirection { UpLeft, UpRight, Left, Right };

public class RandomRoomGenerator
{
    private int roomCount;
    private int jumpHeight;
    private int dashWidth;

    public RandomRoomGenerator(int roomCount, int jumpHeight, int dashWidth)
    {
        this.roomCount = roomCount;
        this.jumpHeight = jumpHeight;
        this.dashWidth = dashWidth;
    }

    // 랜덤한 크기의 방들을 생성하고 방들의 위치정보를 반환하는 함수
    public List<RRoomNode> CalculateDungeon(int roomCount, int roomWidthMin, int roomHeightMin, int roomWidthMax, int roomHeightMax)
    {
        // 첫번째 방 생성
        List<RRoomNode> listOfRoomNode = new List<RRoomNode>();
        Vector2Int currentButtomLeft = new Vector2Int(0, 0);
        Vector2Int currentTopRight = new Vector2Int(
            Random.Range(roomWidthMin, roomWidthMax + 1),
            Random.Range(roomHeightMin, roomHeightMax + 1));
        RRoomNode currentRoom = new RRoomNode(currentButtomLeft, currentTopRight);
        listOfRoomNode.Add(currentRoom);
        roomCount--;

        while (roomCount > 0)
        {
            currentRoom = listOfRoomNode[listOfRoomNode.Count - 1];
            // 방 생성 알고리즘
            listOfRoomNode.Add(CalculateRoomSize(listOfRoomNode, currentRoom, 
                roomWidthMin, roomHeightMin, roomWidthMax, roomHeightMax));
            roomCount--;
        }

        for(int i = 0; i < listOfRoomNode.Count; ++i)
        {
            Debug.Log($"Room{i} : BL {listOfRoomNode[i].BottomLeft} , TR {listOfRoomNode[i].TopRight}");
        }

        return listOfRoomNode;
    }

    // 방 노드의 규모와 위치를 정하고 반환하는 함수
    private RRoomNode CalculateRoomSize(List<RRoomNode> listOfRoomNode, RRoomNode currentNode, int roomWidthMin, int roomHeightMin, int roomWidthMax, int roomHeightMax)
    {
        Vector2Int newBottomLeft = new Vector2Int();
        Vector2Int newTopRight = new Vector2Int();
        NextRoomDirection direction = (NextRoomDirection)Random.Range(0, 4);

        bool isOverlap;
        do
        {
            // 방 생성 방향에 따라 랜덤한 크기의 새로운 방을 생성하는 알고리즘
            switch (direction)
            {
                case NextRoomDirection.UpLeft:
                    newTopRight.x = Random.Range(currentNode.BottomLeft.x + dashWidth, (int)currentNode.Center.x + 1);
                    newTopRight.y = Random.Range(currentNode.TopRight.y + roomHeightMin, currentNode.TopRight.y + roomHeightMax + 1);
                    newBottomLeft.x = Random.Range(newTopRight.x - roomWidthMax, newTopRight.x - roomWidthMin + 1);
                    newBottomLeft.y = currentNode.TopRight.y;
                    break;
                case NextRoomDirection.UpRight:
                    newBottomLeft.x = Random.Range((int)currentNode.Center.x, currentNode.TopRight.x - dashWidth + 1);
                    newBottomLeft.y = currentNode.TopRight.y;
                    newTopRight.x = Random.Range(newBottomLeft.x + roomWidthMin, newBottomLeft.x + roomWidthMax + 1);
                    newTopRight.y = Random.Range(currentNode.TopRight.y + roomHeightMin, currentNode.TopRight.y + roomHeightMax + 1);
                    break;
                case NextRoomDirection.Left:
                    newBottomLeft.x = Random.Range(currentNode.BottomLeft.x - roomWidthMax, currentNode.BottomLeft.x - roomWidthMin + 1);
                    newBottomLeft.y = Random.Range(currentNode.BottomLeft.y, currentNode.TopRight.y - jumpHeight + 1);
                    newTopRight.x = currentNode.BottomLeft.x;
                    newTopRight.y = Random.Range(newBottomLeft.y + roomHeightMin, newBottomLeft.y + roomHeightMax + 1);
                    break;
                case NextRoomDirection.Right:
                    newBottomLeft.x = currentNode.TopRight.x;
                    newBottomLeft.y = Random.Range(currentNode.BottomLeft.y, currentNode.TopRight.y - jumpHeight + 1);
                    newTopRight.x = Random.Range(newBottomLeft.x + roomWidthMin, newBottomLeft.x + roomWidthMax + 1);
                    newTopRight.y = Random.Range(newBottomLeft.y + roomHeightMin, newBottomLeft.y + roomHeightMax + 1);
                    break;
                default:
                    break;
            }
            if (DoOverlap(listOfRoomNode, newBottomLeft, newTopRight))
            {
                Debug.Log("Overlapped");
                isOverlap = true;
                if (direction == NextRoomDirection.Right) direction = NextRoomDirection.UpLeft;
                else ++direction;
            }
            else isOverlap = false;
        }
        while (isOverlap);

        return new RRoomNode(newBottomLeft, newTopRight);
    }

    // 새로 생성한 방이 기존 방과 겹치는지 확인하는 함수
    public bool DoOverlap(List<RRoomNode> listOfRoomNode, Vector2Int rect1BottomLeft, Vector2Int rect1TopRight)
    {
        foreach (var rect2 in listOfRoomNode)
        {
            // x축 겹침 확인
            bool xOverlap = rect1BottomLeft.x < rect2.TopRight.x - 1 && rect1TopRight.x > rect2.BottomLeft.x + 1;

            // y축 겹침 확인
            bool yOverlap = rect1BottomLeft.y < rect2.TopRight.y && rect1TopRight.y > rect2.BottomLeft.y;

            // x축과 y축 모두 겹치면 두 직사각형이 겹친다
            if(xOverlap && yOverlap) return true;
        }
        return false;
    }
}