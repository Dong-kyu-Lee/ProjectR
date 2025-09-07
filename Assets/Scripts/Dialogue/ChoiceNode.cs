using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Choice (Yes/No) Node")]
public class ChoiceNode : DialogueNode
{
    [TextArea] public string question;
    public string yesText = "좋다.";
    public string noText = "싫다.";

    public string nextIfYesId;
    public string nextIfNoId;

    public override string GetNextId(int choiceIndex = 0)
    {
        return (choiceIndex == 0) ? nextIfYesId : nextIfNoId;
    }
}
