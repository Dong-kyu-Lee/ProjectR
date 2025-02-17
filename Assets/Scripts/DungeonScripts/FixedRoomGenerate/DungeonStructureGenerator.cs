using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonStructureGenerator
{
    private int dungeonRow;
    private int dungeonColumn;

    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    private List<Tuple<int, int>> path;
    private bool[,] visited; // 각 방 위치를 방문했는지에 대한 bool값
    private readonly short[] dx = { 1, -1, 0, 0 };
    private readonly short[] dy = { 0, 0, 1, -1 };

    private Vector2Int playerSpawnPoint;
    private Vector2Int finishSpotPoint;
    public Vector2Int PlayerSpawnPoint { get => playerSpawnPoint; }
    public Vector2Int FinishSpotPoint { get => finishSpotPoint; }

    public DungeonStructureGenerator(int dungeonRow, int dungeonColumn)
    {
        this.dungeonRow = dungeonRow;
        this.dungeonColumn = dungeonColumn;
        this.visited = new bool[dungeonRow, dungeonColumn]; // [y,x]
        this.path = new List<Tuple<int, int>>();
    }

    // 생성한 경로를 통해 방의 위치와 열려야 할 문 데이터를 가진 RoomNode 클래스로 바꿔 리턴한다.
    public List<RoomNode> GetDungeonStructure()
    {
        path = FindPath();
        if (path == null)
        {
            Debug.LogWarning("경로를 생성할 수 없음");
            return null;
        }
        playerSpawnPoint = new Vector2Int(path[0].Item2, path[0].Item1);
        finishSpotPoint = new Vector2Int(path[path.Count - 1].Item2, path[path.Count - 1].Item1);

        List<RoomNode> nodeList = new List<RoomNode>();
        for(int i = 0; i < path.Count; ++i)
        {
            RoomNode node = new RoomNode(new Vector2Int(path[i].Item2, path[i].Item1));
            if(i != 0) node.CalculateWhichGateNeedOpen(new Vector2Int(path[i - 1].Item2, path[i - 1].Item1));
            if(i != path.Count - 1) node.CalculateWhichGateNeedOpen(new Vector2Int(path[i + 1].Item2, path[i + 1].Item1));
            nodeList.Add(node);
        }
        return nodeList;
    }

    // 해밀턴 경로 알고리즘을 활용해 방들의 경로를 생성한다.
    private List<Tuple<int,int>> FindPath()
    {
        int r = Random.Range(0, dungeonRow);
        int c = Random.Range(0, dungeonColumn);
        if(DFS(r, c))
        {
            return path;
        }
        else
        {
            for (int i = 0; i < dungeonRow; ++i)
            {
                for (int j = 0; j < dungeonColumn; ++j)
                {
                    if (DFS(i, j))
                    {
                        return path;
                    }
                }
            }
        }
        
        return null; // 해밀턴 경로가 존재하지 않는 경우
    }

    private bool DFS(int i, int j)
    {
        visited[i, j] = true;
        path.Add(new Tuple<int, int>(i, j));

        if (path.Count == dungeonRow * dungeonColumn)
        {
            return true;
        }

        int random = Random.Range(0, 4);
        for (int k = 0; k < 4; ++k)
        {
            int ni = i + dy[(k + random) % 4];
            int nj = j + dx[(k + random) % 4];

            if (IsValid(ni, nj) && !visited[ni, nj])
            {
                if (DFS(ni, nj))
                {
                    return true;
                }
            }
        }

        visited[i, j] = false;
        path.RemoveAt(path.Count - 1);
        return false;
    }

    private bool IsValid(int i, int j)
    {
        return i >= 0 && i < dungeonRow && j >= 0 && j < dungeonColumn;
    }

    // 현재 방 위치에서 이동 가능한 방(자식)을 탐색 후 큐에 삽입하는 함수
    private void EnqueueSuitableChildRoom(Vector2Int room, List<List<RoomNode>> roomNodes)
    {
        int newRoomX = 0, newRoomY = 0;
        // 맵 형태가 획일화 되는 것을 방지하기 위해 큐에 노드를 삽입하는 순서를 렌덤으로 조정함
        int isClockWise = Random.Range(0, 2);
        if (isClockWise == 1)
        {
            for (int i = 0; i < 4; ++i)
            {
                newRoomX = room.x + dx[i];
                newRoomY = room.y + dy[i];
                if (newRoomX >= dungeonColumn || newRoomX < 0 ||
                    newRoomY >= dungeonRow || newRoomY < 0)
                    continue;
                if (!visited[newRoomY, newRoomX])
                {
                    visited[newRoomY, newRoomX] = true;
                    Vector2Int nextRoomPosition = new Vector2Int(newRoomX, newRoomY);
                    // 큐에 다음 노드 좌표(A) 삽입
                    queue.Enqueue(nextRoomPosition);
                    // 현재 노드의 자식 노드 좌표로 A 삽입
                    roomNodes[room.y][room.x].AddChildrenRoomPosition(nextRoomPosition);
                    // A좌표에 있는 룸의 부모를 현재 노드에 있는 룸으로 설정
                    roomNodes[nextRoomPosition.y][nextRoomPosition.x].ParentRoomPosition =
                        room;
                }
            }
        }
        else
        {
            for (int i = 3; i >= 0; --i)
            {
                newRoomX = room.x + dx[i];
                newRoomY = room.y + dy[i];
                if (newRoomX >= dungeonColumn || newRoomX < 0 ||
                    newRoomY >= dungeonRow || newRoomY < 0)
                    continue;
                if (!visited[newRoomY, newRoomX])
                {
                    visited[newRoomY, newRoomX] = true;
                    Vector2Int nextRoomPosition = new Vector2Int(newRoomX, newRoomY);
                    // 큐에 다음 노드 좌표(A) 삽입
                    queue.Enqueue(nextRoomPosition);
                    // 현재 노드의 자식 노드 좌표로 A 삽입
                    roomNodes[room.y][room.x].AddChildrenRoomPosition(nextRoomPosition);
                    // A좌표에 있는 룸의 부모를 현재 노드에 있는 룸으로 설정
                    roomNodes[nextRoomPosition.y][nextRoomPosition.x].ParentRoomPosition =
                        room;
                }
            }
        }
    }
}