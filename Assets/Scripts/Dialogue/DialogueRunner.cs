using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System; // Action을 쓰기 위해 필요

public class DialogueRunner : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private DialogueUI ui;

    [Header("설정")]
    [SerializeField] private KeyCode nextKey = KeyCode.E;
    [SerializeField] private bool useMouseClick = true;

    public bool IsRunning { get; private set; }

    private Coroutine routine;

    public void Run(DialogueGraph graph, Action<string> onEvent = null)
    {
        if (IsRunning || graph == null || ui == null) return;
        routine = StartCoroutine(RunRoutine(graph, onEvent));
    }

    // 대화를 강제로 중단하는 함수 (외부에서 호출 가능; 프롤로그 스킵)
    public void Stop()
    {
        if (!IsRunning) return;

        // RunRoutine 안에서 중첩 실행 중인 WaitForInput까지 함께 정리
        StopAllCoroutines();
        routine = null;

        ui.Close();
        IsRunning = false;
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
        routine = null;
    }

    private IEnumerator WaitForInput()
    {
        yield return null; // 이전 프레임의 클릭 입력이 중복 감지되는 것을 방지
        while (true)
        {
            // 스킵 버튼 / 선택지 버튼 위에서의 클릭은 대화 진행 입력으로 처리하지 않는다.
            // (대화창 배경 위 클릭은 기존처럼 다음 대사로 넘어간다)
            if (Input.GetKeyDown(nextKey) || (useMouseClick && Input.GetMouseButtonDown(0) && !IsPointerOverButton()))
            {
                if(ui.isActiveAndEnabled == true)
                    if (!ui.TrySkipTyping()) break;
            }
            yield return null;
        }
    }

    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    // 마우스 포인터가 Button 위에 있는지 검사
    private bool IsPointerOverButton()
    {
        EventSystem es = EventSystem.current;
        if (es == null || !es.IsPointerOverGameObject()) return false;

        PointerEventData data = new PointerEventData(es) { position = Input.mousePosition };
        raycastResults.Clear();
        es.RaycastAll(data, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.GetComponentInParent<Button>() != null) return true;
        }
        return false;
    }
}