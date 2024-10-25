using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitioner
{
    private RoomNode rootNode;
    public RoomNode RootNode { get => rootNode; }

    public BinarySpacePartitioner(int dungeonWidth, int dungeonLength)
    {
        rootNode = new RoomNode(new Vector2Int(0,0), new Vector2Int(dungeonWidth, dungeonLength), null, 0);
    }

    // 노드를 둘로 랜덤한 크기로 쪼개며 더이상 쪼갤 수 없을 때까지 반복하고 리스트에 저장하는 함수
    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<RoomNode> queue = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        queue.Enqueue(rootNode);
        listToReturn.Add(rootNode);
        int iteration = 0;
        while (iteration < maxIterations && queue.Count > 0)
        {
            iteration++;
            RoomNode currentNode = queue.Dequeue();
            if (currentNode.Width >= roomWidthMin * 2 && currentNode.Length >= roomLengthMin * 2)
            {
                SplitTheSpace(currentNode, listToReturn, roomWidthMin, roomLengthMin, queue);
            }
        }

        return listToReturn;
    }

    // 노드의 공간을 자른 후 리스트에 집어넣는 함수
    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomWidthMin, int roomLengthMin, Queue<RoomNode> queue)
    {
        Line line = GetLineDividingSpace(currentNode.BottomLeftAreaCorner, currentNode.TopRightAreaCorner,
            roomWidthMin, roomLengthMin);

        RoomNode node1, node2;
        if (line.Orientation == Orientation.Horizontal)
        {
            // 아래 노드
            node1 = new RoomNode(
                currentNode.BottomLeftAreaCorner,
                new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinate.y),
                currentNode,
                currentNode.TreeLayerIndex + 1);
            // 위 노드
            node2 = new RoomNode(
                new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinate.y),
                currentNode.TopRightAreaCorner,
                currentNode,
                currentNode.TreeLayerIndex + 1);
        }
        else
        {
            // 왼쪽 노드
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner,
                new Vector2Int(line.Coordinate.x, currentNode.TopRightAreaCorner.y),
                currentNode,
                currentNode.TreeLayerIndex + 1);
            // 오른쪽 노드
            node2 = new RoomNode(new Vector2Int(line.Coordinate.x, currentNode.BottomLeftAreaCorner.y),
                currentNode.TopRightAreaCorner,
                currentNode,
                currentNode.TreeLayerIndex + 1);
        }

        listToReturn.Add(node1);
        queue.Enqueue(node1);
        listToReturn.Add(node2);
        queue.Enqueue(node2);
    }

    // 노드를 자를 기준선을 설정하는 함수
    private Line GetLineDividingSpace(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;
        // 높이 혹은 너비를 반으로 잘랐을 때 방 최소 높이/너비보다 낮지 않는지를 나타내는 bool변수
        bool lengthState = (topRightAreaCorner.y - bottomLeftAreaCorner.y) / 2 >= roomLengthMin;
        bool widthState = (topRightAreaCorner.x - bottomLeftAreaCorner.x) / 2 >= roomWidthMin;

        // 높이/너비 둘 다 충족 => 렌덤
        if (lengthState && widthState)
        {
            orientation = (Orientation)Random.Range(0, 2);
        }
        else if(widthState) // 너비만 충족 => 세로로 자름
        {
            orientation = Orientation.Vertical;
        }
        else // 높이만 충족 => 가로로 자름
        {
            orientation =Orientation.Horizontal;
        }

        return new Line(orientation, GetCoordinateForOrientation(
            orientation, bottomLeftAreaCorner, topRightAreaCorner,
            roomWidthMin, roomLengthMin));
    }

    // 라인의 방향에 따른 라인의 위치를 렌덤으로 결정하는 함수
    private Vector2Int GetCoordinateForOrientation(Orientation orientation, Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        if (orientation == Orientation.Horizontal)
        {
            return new Vector2Int(0, Random.Range(bottomLeftAreaCorner.y + roomLengthMin,
                topRightAreaCorner.y - roomLengthMin));
        }
        else
        {
            return new Vector2Int(Random.Range(bottomLeftAreaCorner.x + roomWidthMin,
                topRightAreaCorner.x - roomWidthMin), 0);
        }
    }
}