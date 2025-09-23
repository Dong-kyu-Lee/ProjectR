using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private ConversationUIController ui;
    public bool IsRunning { get; private set; }

    // 그래프 실행 시작. 이미 실행 중이면 무시.
    public void Run(DialogueGraph graph, string overrideStartId = null)
    {
        if (IsRunning || graph == null || ui == null) return;
        StartCoroutine(RunRoutine(graph, overrideStartId));
    }

    private IEnumerator RunRoutine(DialogueGraph graph, string overrideStartId)
    {
        IsRunning = true;

        string currentId = string.IsNullOrEmpty(overrideStartId) ? graph.startNodeId : overrideStartId;
        DialogueNode node = graph.Get(currentId);

        while (node != null)
        {
            if (node is SayNode s)
            {
                bool ok = false;
                ui.OpenSingle(
                    s.text,
                    () => ok = true,
                    string.IsNullOrEmpty(s.okText) ? "다음" : s.okText
                );
                yield return new WaitUntil(() => ok);
                node = graph.Get(s.GetNextId());
            }
            else if (node is ChoiceNode c)
            {
                int choice = -1; // 0: 예, 1: 아니오
                ui.OpenChoices(
                    c.question,
                    yes: () => choice = 0,
                    no: () => choice = 1,
                    c.yesText, c.noText
                );
                yield return new WaitUntil(() => choice >= 0);
                node = graph.Get(c.GetNextId(choice));
            }
            else if (node is EventNode e)
            {
                e.Invoke();                 // 특성창 열기 등
                node = graph.Get(e.GetNextId());
                yield return null;          // 다음 프레임
            }
            else
            {
                break; // 알 수 없는 노드 → 종료
            }
        }

        //UI가 남아있으면 닫기
        if (ui != null && ui.IsOpen) ui.Close();

        IsRunning = false;
    }
}
