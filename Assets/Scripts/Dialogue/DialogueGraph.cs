using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Graph")]
public class DialogueGraph : ScriptableObject
{
    [Header("시작 노드 ID")]
    public string startNodeId = "Start";

    [Header("그래프에 포함될 노드들")]
    public DialogueNode[] nodes;

    private Dictionary<string, DialogueNode> dict;

    private void OnEnable() => BuildDict();

    public void BuildDict()
    {
        dict = new Dictionary<string, DialogueNode>();
        if (nodes == null) return;

        foreach (var n in nodes)
        {
            if (n == null || string.IsNullOrEmpty(n.nodeId)) continue;
            dict[n.nodeId] = n; // 동일 ID면 마지막 등록이 우선
        }
    }

    public DialogueNode Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (dict == null || dict.Count == 0) BuildDict();
        dict.TryGetValue(id, out var node);
        return node;
    }
}
