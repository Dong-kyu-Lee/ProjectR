using System.Collections;
using UnityEngine;
using System; // Action을 쓰기 위해 필요

public class DialogueRunner : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private DialogueUI ui;

    [Header("설정")]
    [SerializeField] private KeyCode nextKey = KeyCode.E;
    [SerializeField] private bool useMouseClick = true;

    public bool IsRunning { get; private set; }

    public void Run(DialogueGraph graph, Action<string> onEvent = null)
    {
        if (IsRunning || graph == null || ui == null) return;
        StartCoroutine(RunRoutine(graph, onEvent));
    }

    private IEnumerator RunRoutine(DialogueGraph graph, Action<string> onEvent)
    {
        IsRunning = true;
        ui.Open();

        DialogueNode node = graph.Get(graph.startNodeId);

        while (node != null)
        {
            if (node is SayNode sayNode)
            {
                ui.SetSayNode(sayNode);
                yield return StartCoroutine(WaitForInput());
                node = graph.Get(sayNode.GetNextId());
            }
            else if (node is ChoiceNode choiceNode)
            {
                int selectedIndex = -1;
                ui.SetChoiceNode(choiceNode,
                    onYes: () => selectedIndex = 0,
                    onNo: () => selectedIndex = 1
                );
                yield return new WaitUntil(() => selectedIndex != -1);
                node = graph.Get(choiceNode.GetNextId(selectedIndex));
            }
            else if (node is EventNode eventNode)
            {
                if (!string.IsNullOrEmpty(eventNode.eventName))
                {
                    onEvent?.Invoke(eventNode.eventName);
                }

                node = graph.Get(eventNode.GetNextId());
                yield return null;
            }
            else
            {
                break;
            }
        }

        ui.Close();
        IsRunning = false;
    }

    private IEnumerator WaitForInput()
    {
        while (true)
        {
            if (Input.GetKeyDown(nextKey) || (useMouseClick && Input.GetMouseButtonDown(0)))
            {
                if(ui.isActiveAndEnabled == true)
                    if (!ui.TrySkipTyping()) break;
            }
            yield return null;
        }
    }
}