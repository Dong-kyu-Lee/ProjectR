using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Dialogue/Event Node")]
public class EventNode : DialogueNode
{
    [Tooltip("노드 진입 시 실행할 이벤트(예: 특성창 SetActive(true))")]
    public UnityEvent onInvoke;
    public string nextNodeId;

    public override string GetNextId(int choiceIndex = 0) => nextNodeId;

    public void Invoke() => onInvoke?.Invoke();
}
