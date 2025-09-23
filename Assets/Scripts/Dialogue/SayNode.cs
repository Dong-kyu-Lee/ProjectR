using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Say Node")]
public class SayNode : DialogueNode
{
    [TextArea] public string text;
    public string okText = "다음";
    public string nextNodeId;

    public override string GetNextId(int choiceIndex = 0) => nextNodeId;
}
