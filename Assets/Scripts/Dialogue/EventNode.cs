using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Event Node")]
public class EventNode : DialogueNode
{
    public string eventName; // "OpenUpgrade" 같은 키값
    public string nextNodeId;

    public override string GetNextId(int choiceIndex = 0) => nextNodeId;
}
