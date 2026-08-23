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
    // dx/dy와 같은 순서(오른쪽, 왼쪽, 위, 아래)의 방향별 선택 가중치. (가로로 뻗은 던전을 만들기 위함)
    // 값을 키울수록 해당 방향이 앞쪽 순서로 뽑힐 확률이 올라간다. (모두 1 이상이어야 함)
    private readonly int[] directionWeights = { 3, 3, 1, 1 };

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
        
        int[] directionOrder = GetShuffledDirections();

        for (int k = 0; k < 4; ++k)
        {
            int direction = directionOrder[k];
            int ni = i + dy[direction];
            int nj = j + dx[direction];

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

    // directionWeights를 기반으로 4방향(dx/dy의 인덱스)의 탐색 순서를 섞어 반환한다.
    // 가중치가 클수록 앞쪽 순서에 뽑힐 확률이 높은 비복원 가중치 추출 방식이다.
    private int[] GetShuffledDirections()
    {
        int[] order = { 0, 1, 2, 3 };
        int remainWeight = 0;
        for (int k = 0; k < order.Length; ++k)
        {
            remainWeight += directionWeights[order[k]];
        }

        // order[k]에 k번째로 탐색할 방향을 확정시킨다. 아직 뽑히지 않은 방향은 order[k] 뒤쪽에 남는다.
        // 마지막 하나는 자동으로 결정되므로 order.Length - 1번만 반복한다.
        for (int k = 0; k < order.Length - 1; ++k)
        {
            int pick = Random.Range(0, remainWeight);
            int index = k;
            while (index < order.Length - 1 && pick >= directionWeights[order[index]])
            {
                pick -= directionWeights[order[index]];
                ++index;
            }

            // 뽑힌 방향을 k번째 자리로 옮기고, 원래 k번째에 있던 방향은 후보로 되돌린다.
            int picked = order[index];
            order[index] = order[k];
            order[k] = picked;

            remainWeight -= directionWeights[picked];
        }

        return order;
    }
}