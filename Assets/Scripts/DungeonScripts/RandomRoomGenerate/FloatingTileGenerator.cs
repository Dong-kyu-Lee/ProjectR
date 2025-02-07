using System.Collections.Generic;
using UnityEngine;

public struct FloatingTile
{
    public Vector2Int left;
    public Vector2Int right;
    public Vector2Int center;

    public FloatingTile(Vector2Int left, Vector2Int right)
    {
        this.left = left;
        this.right = right;
        this.center = new Vector2Int((left.x + right.x) / 2, (left.y + right.y) / 2);
    }
}

public class FloatingTileGenerator
{
    private int jumpHeight;
    private int dashWidth;
    private int floatingTileWidthMin;
    private int floatingTileWidthMax;

    public FloatingTileGenerator(int jumpHeight, int dashWidth, int floatingTileWidthMin, int floatingTileWidthMax)
    {
        this.jumpHeight = jumpHeight;
        this.dashWidth = dashWidth;
        this.floatingTileWidthMin = floatingTileWidthMin;
        this.floatingTileWidthMax = floatingTileWidthMax;
    } 

    // 공중 발판 리스트를 알고리즘에 따라 생성하는 함수
    public List<FloatingTile> GenerateFloatingTile(List<RRoomNode> listOfRoom)
    {
        List<FloatingTile> listOfTile = new List<FloatingTile>();
        for(int i = 0; i < listOfRoom.Count; ++i)
        {
            if (i == listOfRoom.Count - 1) { break; } // 마지막 방일 때

            // 필수 발판 생성 (다음 방의 높이가 점프로 2번 이상 뛰어야 되는 경우)
            if(listOfRoom[i+1].BottomLeft.y - listOfRoom[i].BottomLeft.y > jumpHeight)
            {
                // 생성되어야 하는 필수 발판의 수
                int numberOffloatingTile = (listOfRoom[i + 1].BottomLeft.y - listOfRoom[i].BottomLeft.y) / jumpHeight;
                // 위에서부터 아래로 생성한다. (이유 : 다음 방에 도달할 수 있게 해주는 발판의 x좌표를 먼저 잡아야하기 때문)
                int y = listOfRoom[i + 1].BottomLeft.y - jumpHeight;
                listOfTile.Add(GenerateHeighestFloatingTile(listOfRoom[i], listOfRoom[i + 1], y));
                int currentTileHeight = y;

                while(currentTileHeight - jumpHeight > listOfRoom[i].BottomLeft.y + 2)
                {
                    int leftX, rightX; // 발판의 왼쪽 끝 x값, 오른쪽 끝 x값
                    int minLimit, maxLimit; // 발판이 방 경계를 벗어나지 않는 선에서 가지는 최소, 최대 길이
                    FloatingTile preTile = listOfTile[listOfTile.Count - 1];
                    // 직전 타일이 방의 오른쪽에 위치해 있을 때
                    if (preTile.center.x > listOfRoom[i].Center.x)
                    {
                        rightX = Random.Range(preTile.left.x - dashWidth, preTile.left.x - 3);
                        minLimit = (rightX - listOfRoom[i].BottomLeft.x - 1 < floatingTileWidthMin)
                            ? rightX - listOfRoom[i].BottomLeft.x - 1 : floatingTileWidthMin;
                        maxLimit = (rightX - listOfRoom[i].BottomLeft.x - 1 < floatingTileWidthMax)
                            ? rightX - listOfRoom[i].BottomLeft.x - 1 : floatingTileWidthMax;
                        leftX = Random.Range(rightX - maxLimit, rightX - minLimit + 1);
                    }
                    else // 왼쪽에 위치해 있을 때
                    {
                        leftX = Random.Range(preTile.right.x + 3, preTile.right.x + dashWidth);
                        minLimit = (listOfRoom[i].TopRight.x - leftX - 1 < floatingTileWidthMin)
                            ? listOfRoom[i].TopRight.x - leftX - 1 : floatingTileWidthMin;
                        maxLimit = (listOfRoom[i].TopRight.x - leftX - 1 < floatingTileWidthMax)
                            ? listOfRoom[i].TopRight.x - leftX - 1 : floatingTileWidthMax;
                        rightX = Random.Range(leftX + minLimit, leftX + maxLimit + 1);
                    }

                    currentTileHeight -= jumpHeight;
                    listOfTile.Add(new FloatingTile(new Vector2Int(leftX, currentTileHeight), new Vector2Int(rightX, currentTileHeight)));
                }
            }
            
            // 선택 발판 생성 (다음 방의 높이가 점프 한번으로도 충분히 건너는 경우)

        }

        return listOfTile;
    }

    // 다음 방으로 이동하기 위해 필요한 가장 높이있는 발판을 생성하는 함수 (첫 필수발판 생성)
    // 다음 방의 방향(NextRoomDirection)에 따라 첫 필수발판 생성 알고리즘이 다름
    private FloatingTile GenerateHeighestFloatingTile(RRoomNode currentRoom, RRoomNode nextRoom, int y)
    {
        // 다음 방의 방향 결정
        int leftX, rightX;
        if (currentRoom.Center.x < nextRoom.Center.x) // right
        {
            rightX = Random.Range(currentRoom.TopRight.x, currentRoom.TopRight.x - dashWidth);
            leftX = Random.Range(rightX - floatingTileWidthMax, rightX - floatingTileWidthMin + 1);
        }
        else // left
        {
            leftX = Random.Range(currentRoom.BottomLeft.x, currentRoom.BottomLeft.x + dashWidth);
            rightX = Random.Range(leftX + floatingTileWidthMin, leftX + floatingTileWidthMax + 1);
        }

        FloatingTile tile = new FloatingTile(new Vector2Int(leftX, y), new Vector2Int(rightX, y));
        return tile;
    }
}