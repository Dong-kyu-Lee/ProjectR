using System.Collections.Generic;
using UnityEngine;

// StageData의 노드 목록을 탐색하는 런타임 그래프
// 노드는 id로 서로를 참조하며(인접 리스트), 나가는 간선이 없는 노드에 도달하면 스테이지가 끝난다.
// 사이클은 허용하지 않는다. (스테이지는 반드시 끝나야 하므로 DAG 구조)
public class StageGraph
{
    private readonly Dictionary<string, StageNode> lookup = new Dictionary<string, StageNode>();
    private readonly string stageName;

    public StageNode StartNode { get; private set; }

    public StageGraph(StageData stageData)
    {
        stageName = stageData.name;

        List<StageNode> nodes = stageData.nodes;
        string startNodeId = stageData.startNodeId;

        // 노드가 설정되지 않은 기존 StageData는 기존 고정 흐름으로 동작
        if (nodes == null || nodes.Count == 0)
        {
            nodes = CreateDefaultNodes();
            startNodeId = nodes[0].id;
        }

        foreach (StageNode node in nodes)
        {
            if (node == null || string.IsNullOrEmpty(node.id)) continue;
            if (lookup.ContainsKey(node.id))
            {
                Debug.LogError($"[{stageName}] 중복된 노드 id: {node.id}");
                continue;
            }
            lookup.Add(node.id, node);
        }

        StartNode = GetNode(startNodeId);
        if (StartNode == null)
            Debug.LogError($"[{stageName}] 시작 노드를 찾을 수 없음: {startNodeId}");
    }

    public StageNode GetNode(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        lookup.TryGetValue(id, out StageNode node);
        return node;
    }

    // 이동 가능한 간선 목록을 반환하는 함수. 대상 노드가 없는 간선은 제외한다.
    public List<StageEdge> GetValidEdges(StageNode node)
    {
        List<StageEdge> validEdges = new List<StageEdge>();
        if (node == null || node.edges == null) return validEdges;

        foreach (StageEdge edge in node.edges)
        {
            if (edge == null) continue;
            if (lookup.ContainsKey(edge.targetNodeId) == false)
            {
                Debug.LogError($"[{stageName}] '{node.id}' 노드의 간선이 존재하지 않는 노드를 가리킴: {edge.targetNodeId}");
                continue;
            }
            validEdges.Add(edge);
        }
        return validEdges;
    }

    // 현재 노드에서 다음 노드를 선택하는 함수. 종료 노드라면 null을 반환한다.
    // choiceIndex : PlayerChoice 모드에서 플레이어가 고른 간선 인덱스 (GetValidEdges 기준)
    public StageNode SelectNext(StageNode node, int choiceIndex = -1)
    {
        List<StageEdge> edges = GetValidEdges(node);
        if (edges.Count == 0) return null;

        StageEdge selected;
        switch (node.branchMode)
        {
            case StageBranchMode.Random:
                selected = PickWeightedRandom(edges);
                break;
            case StageBranchMode.PlayerChoice:
                if (choiceIndex >= 0 && choiceIndex < edges.Count)
                {
                    selected = edges[choiceIndex];
                }
                else
                {
                    Debug.LogWarning($"[{stageName}] '{node.id}' 노드의 선택값이 없어 첫 번째 간선으로 이동함");
                    selected = edges[0];
                }
                break;
            default:
                selected = edges[0];
                break;
        }
        return GetNode(selected.targetNodeId);
    }

    private StageEdge PickWeightedRandom(List<StageEdge> edges)
    {
        float totalWeight = 0f;
        foreach (StageEdge edge in edges)
            totalWeight += Mathf.Max(0f, edge.weight);

        // 가중치가 모두 0이면 균등 확률로 선택
        if (totalWeight <= 0f)
            return edges[Random.Range(0, edges.Count)];

        float pick = Random.Range(0f, totalWeight);
        foreach (StageEdge edge in edges)
        {
            pick -= Mathf.Max(0f, edge.weight);
            if (pick < 0f) return edge;
        }
        return edges[edges.Count - 1];
    }

    // 기존 StageFlow enum과 동일한 고정 흐름
    // Normal1 -> Normal2 -> MiddleBoss -> Normal3 -> Shop -> FinalBoss
    public static List<StageNode> CreateDefaultNodes()
    {
        StageNode middleBoss = new StageNode("mid_boss", StageNodeType.MiddleBoss, "normal_3");
        middleBoss.storyID = StoryID.Temp_Middle_Boss;
        StageNode finalBoss = new StageNode("final_boss", StageNodeType.FinalBoss);
        finalBoss.storyID = StoryID.Temp_Final_Boss;

        return new List<StageNode>
        {
            new StageNode("normal_1", StageNodeType.NormalDungeon, "normal_2"),
            new StageNode("normal_2", StageNodeType.NormalDungeon, "mid_boss"),
            middleBoss,
            new StageNode("normal_3", StageNodeType.NormalDungeon, "shop"),
            new StageNode("shop", StageNodeType.Shop, "final_boss"),
            finalBoss,
        };
    }

    // 그래프 설정 오류를 검사하는 함수. 오류 메시지 목록을 반환하며 비어 있으면 정상이다.
    public static List<string> Validate(string startNodeId, List<StageNode> nodes)
    {
        List<string> errors = new List<string>();
        if (nodes == null || nodes.Count == 0) return errors; // 기본 흐름 사용

        // 1. id 유효성 / 중복
        Dictionary<string, StageNode> map = new Dictionary<string, StageNode>();
        for (int i = 0; i < nodes.Count; ++i)
        {
            StageNode node = nodes[i];
            if (node == null) continue;
            if (string.IsNullOrEmpty(node.id))
            {
                errors.Add($"{i}번 노드의 id가 비어 있음");
                continue;
            }
            if (map.ContainsKey(node.id))
            {
                errors.Add($"중복된 노드 id: {node.id}");
                continue;
            }
            map.Add(node.id, node);
        }

        // 2. 시작 노드 존재
        if (string.IsNullOrEmpty(startNodeId) || map.ContainsKey(startNodeId) == false)
            errors.Add($"시작 노드를 찾을 수 없음: '{startNodeId}'");

        // 3. 간선 대상 존재 / 가중치
        foreach (StageNode node in map.Values)
        {
            if (node.edges == null) continue;
            float totalWeight = 0f;
            foreach (StageEdge edge in node.edges)
            {
                if (edge == null) continue;
                if (string.IsNullOrEmpty(edge.targetNodeId) || map.ContainsKey(edge.targetNodeId) == false)
                    errors.Add($"'{node.id}' 노드의 간선이 존재하지 않는 노드를 가리킴: '{edge.targetNodeId}'");
                totalWeight += Mathf.Max(0f, edge.weight);
            }
            if (node.branchMode == StageBranchMode.Random && node.edges.Count > 1 && totalWeight <= 0f)
                errors.Add($"'{node.id}' 노드는 Random 모드지만 간선 가중치 합이 0임");
        }

        // 4. 사이클 검사 (DFS, 0: 미방문 / 1: 방문 중 / 2: 완료)
        Dictionary<string, int> visitState = new Dictionary<string, int>();
        foreach (string id in map.Keys)
        {
            if (FindCycle(id, map, visitState, out string cycleNodeId))
            {
                errors.Add($"사이클이 존재함: '{cycleNodeId}' 노드로 되돌아오는 경로가 있음");
                break;
            }
        }

        // 5. 시작 노드에서 도달할 수 없는 노드
        if (map.TryGetValue(startNodeId ?? string.Empty, out StageNode startNode))
        {
            HashSet<string> reachable = new HashSet<string>();
            Queue<StageNode> queue = new Queue<StageNode>();
            reachable.Add(startNode.id);
            queue.Enqueue(startNode);
            while (queue.Count > 0)
            {
                StageNode current = queue.Dequeue();
                if (current.edges == null) continue;
                foreach (StageEdge edge in current.edges)
                {
                    if (edge == null || map.TryGetValue(edge.targetNodeId ?? string.Empty, out StageNode next) == false) continue;
                    if (reachable.Add(next.id)) queue.Enqueue(next);
                }
            }
            foreach (string id in map.Keys)
            {
                if (reachable.Contains(id) == false)
                    errors.Add($"시작 노드에서 도달할 수 없는 노드: '{id}'");
            }
        }

        return errors;
    }

    private static bool FindCycle(string id, Dictionary<string, StageNode> map, Dictionary<string, int> visitState, out string cycleNodeId)
    {
        cycleNodeId = null;
        visitState.TryGetValue(id, out int state);
        if (state == 2) return false;
        if (state == 1)
        {
            cycleNodeId = id;
            return true;
        }

        visitState[id] = 1;
        StageNode node = map[id];
        if (node.edges != null)
        {
            foreach (StageEdge edge in node.edges)
            {
                if (edge == null || string.IsNullOrEmpty(edge.targetNodeId) || map.ContainsKey(edge.targetNodeId) == false) continue;
                if (FindCycle(edge.targetNodeId, map, visitState, out cycleNodeId)) return true;
            }
        }
        visitState[id] = 2;
        return false;
    }
}
