using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonStructureGenerator
{
    private int dungeonRow;
    private int dungeonColumn;

    private bool[,] visited; // 각 방 위치를 방문했는지에 대한 bool값
    private readonly short[] dx = { 1, -1, 0, 0 };
    private readonly short[] dy = { 0, 0, 1, -1 };

    public DungeonStructureGenerator(int dungeonRow, int dungeonColumn)
    {
        this.dungeonRow = dungeonRow;
        this.dungeonColumn = dungeonColumn;
        visited = new bool[dungeonColumn, dungeonRow];
        // 0 1 2 3
        // 4 5 6 7
        // 8 9 10 11
        // 인덱스 값 / dungeonColumn = RoomColumn
        // 인덱스 값 % dungeonColumn = RoomRow
    }

    // BFS 알고리즘을 활용해 맵 조각을 랜덤하게 연결한다.
    // 수정 : Queue에 RoomNode를 넣지 않고 Vector2Int로 노드의 좌표만 넣는다.
    // 수정 : RoomNode를 미리 생성하고, 이 알고리즘에서 노드 간의 이동 경로가 생성되면
    //        이를 RoomNode의 ChildrenRoomPosition에 작용한다.
    public RoomNode[,] CreateDungeonStructure()
    {
        RoomNode[,] roomNodes = new RoomNode[dungeonColumn, dungeonRow];
        Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
        Vector2Int root = new Vector2Int(0, 0);
        roomQueue.Enqueue(root);
        visited[root.x, root.y] = true;

        while(roomQueue.Count != 0)
        {
            var room = roomQueue.Dequeue();
            bool[] possibleDirections = { true, true, true, true };

            for (int i = 0; i < 4; ++i)
            {
                int newRoomX = room.x + dx[i];
                int newRoomY = room.y + dy[i];
                if (newRoomX >= dungeonColumn || newRoomX < 0 ||
                    newRoomY >= dungeonRow || newRoomY < 0)
                {
                    possibleDirections[i] = false;
                    continue;
                }
                if(visited[newRoomX, newRoomY])
                {
                    possibleDirections[i] = false;
                }
            }

            int isClockWise = Random.Range(0, 2);
            if (isClockWise == 0)
            {
                for (int i = 3; i >= 0; --i)
                {
                    if (possibleDirections[i])
                    {
                        // 큐에 다음 노드 좌표(A) 삽입
                        // 현재 노드의 자식 노드 좌표로 A 삽입
                        // A좌표에 있는 룸의 부모를 현재 노드에 있는 룸으로 설정
                    }
                }
            }
        }

        return roomNodes;
    }
}