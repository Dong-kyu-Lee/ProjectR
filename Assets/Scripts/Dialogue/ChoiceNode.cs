using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Choice(Yes/No)Node")]
public class ChoiceNode : DialogueNode
{
    [Header("화자 설정 (추가됨)")]
    public SpeakerProfile speaker; // 누가 질문하는가?

    [Header("질문 및 선택지")]
    [TextArea] public string question;
    public string yesText = "좋다.";
    public string noText = "싫다.";

    [Header("연결")]
    public string nextIfYesId;
    public string nextIfNoId;

    public override string GetNextId(int choiceIndex = 0)
    {
        return (choiceIndex == 0) ? nextIfYesId : nextIfNoId;
    }
}