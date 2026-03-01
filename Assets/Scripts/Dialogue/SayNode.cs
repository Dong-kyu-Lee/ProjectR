using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Say Node")]
public class SayNode : DialogueNode
{
    [Header("대화 내용")]
    [TextArea(3, 5)] public string text; // 대사

    [Header("화자 설정")]
    public SpeakerProfile speaker; // 누가 말하는지 (이름용)

    [Header("연결")]
    public string nextNodeId; // 다음 대화 ID

    public override string GetNextId(int choiceIndex = 0) => nextNodeId;
}