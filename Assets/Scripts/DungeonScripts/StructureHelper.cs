using System;
using System.Collections.Generic;
using UnityEngine;

public static class StructureHelper
{
    // 트리의 Leaf노드. 즉, room에 해당하는 노드를 가져온다.
    public static List<Node> TraverseGraphToExtractLeafs(RoomNode parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> listToReturn = new List<Node>();
        // 첫 노드가 Leaf노드일 경우. 
        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<Node>() { parentNode };
        }
        // Leaf 노드를 탐색하기 위해 첫 노드의 자식을 큐에 집어넣는다.
        foreach (var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }
        // 큐에 들어간 자식노드 중 Leaf노드(자식이 없음)이면 List에 추가하고 그렇지 않으면
        // 그 노드의 자식 노드를 큐에 집어넣음. (BFS랑 비슷)
        while (nodesToCheck.Count > 0)
        {
            var currentNode = nodesToCheck.Dequeue();
            if (currentNode.ChildrenNodeList.Count == 0)
            {
                listToReturn.Add(currentNode);
            }
            else
            {
                foreach (var child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(child);
                }
            }
        }
        return listToReturn;
    }
}
