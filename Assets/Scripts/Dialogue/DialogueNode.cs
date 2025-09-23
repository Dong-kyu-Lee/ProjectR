using UnityEngine;

public abstract class DialogueNode : ScriptableObject
{
    // 모든 대화 노드의 베이스. ScriptableObject로 만들어 그래프에 담아 사용.
    // 다음 노드의 nodeId를 반환. (choiceIndex: 선택지 인덱스)
    // 없으면 null/빈 문자열을 반환하여 종료.
    [Header("그래프 내 고유 ID")]
    public string nodeId;

    public abstract string GetNextId(int choiceIndex = 0);
}
