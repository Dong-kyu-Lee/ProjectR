using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonStructureGenerator
{
    private int roomCount;

    private List<Tuple<int, int>> path;
    private HashSet<Tuple<int, int>> visitedSet; // 방문한 방의 위치를 저장하는 Set
    private readonly short[] dx = { 1, -1, 0, 0 };
    private readonly short[] dy = { 0, 0, 1, -1 };

    public DungeonStructureGenerator(int roomCount)
    {
        this.roomCount = roomCount;
        this.visitedSet = new HashSet<Tuple<int, int>>();
        this.path = new List<Tuple<int, int>>();
        this.roomCount = roomCount;
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
        if(DFS(0,0))
        {
            return path;
        }
        
        return null; // 해밀턴 경로가 존재하지 않는 경우
    }

    // 주어진 시작점부터 경로를 탐색하고 탐색한 노드가 유효하면 path에 저장
    private bool DFS(int i, int j)
    {
        visitedSet.Add(new Tuple<int, int>(i, j));
        path.Add(new Tuple<int, int>(i, j));

        if (path.Count == roomCount)
        {
            return true;
        }
        
        // random 값은 0~3까지의 랜덤한 정수를 가지는데, 각각의 가중치는 다음과 같다.
        // 0 : 20%, 1 : 30%, 2 : 20%, 3 : 30%
        int[] weightedValues = { 0, 1, 1, 3, 3, 0, 2, 2, 1, 3 };
        int random = weightedValues[Random.Range(0, weightedValues.Length)];

        for (int k = 0; k < 4; ++k)
        {
            int ni = i + dy[(k + random) % 4];
            int nj = j + dx[(k + random) % 4];

            if (!visitedSet.Contains(new Tuple<int, int>(ni, nj)))
            {
                if (DFS(ni, nj))
                {
                    return true;
                }
            }
        }

        visitedSet.Remove(new Tuple<int, int>(i, j));
        path.RemoveAt(path.Count - 1);
        return false;
    }
}