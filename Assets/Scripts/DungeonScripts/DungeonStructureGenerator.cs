using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonStructureGenerator
{
    private int dungeonRow;
    private int dungeonColumn;

    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    private bool[,] visited; // 각 방 위치를 방문했는지에 대한 bool값
    private readonly short[] dx = { 1, -1, 0, 0 };
    private readonly short[] dy = { 0, 0, 1, -1 };

    public DungeonStructureGenerator(int dungeonRow, int dungeonColumn)
    {
        this.dungeonRow = dungeonRow;
        this.dungeonColumn = dungeonColumn;
        visited = new bool[dungeonRow, dungeonColumn]; // [y,x]
    }

    // BFS 알고리즘을 활용해 맵 조각을 랜덤하게 연결한다.
    public List<List<RoomNode>> CreateDungeonStructure()
    {
        List<List<RoomNode>> roomNodes = new List<List<RoomNode>>();
        for(int i = 0; i < dungeonRow; ++i)
        {
            roomNodes.Add(new List<RoomNode>());
            for(int j = 0; j < dungeonColumn; ++j)
            {
                roomNodes[i].Add(new RoomNode(new Vector2Int(0, 0)));
            }
        }
        
        Vector2Int root = new Vector2Int(1, 1);
        queue.Enqueue(root);
        visited[root.y, root.x] = true;

        while(queue.Count != 0)
        {
            var room = queue.Dequeue();
            roomNodes[room.y][room.x].RoomPosition = room;

            EnqueueSuitableChildRoom(room, roomNodes);
        }

        return roomNodes;
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