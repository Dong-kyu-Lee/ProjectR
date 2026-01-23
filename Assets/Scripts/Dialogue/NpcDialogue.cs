using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class NpcDialogue : MonoBehaviour
{
    [Header("대화 데이터")]
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private DialogueGraph graph;

    [Header("트리거 설정")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    // ▼ 핵심: 이벤트 키와 실제 행동을 연결하는 구조체
    [System.Serializable]
    public struct DialogueEvent
    {
        public string eventKey;   // 예: "OpenUpgrade"
        public UnityEvent action; // 예: UpgradeUI.SetActive(true)
    }

    [Header("이 NPC가 처리할 이벤트 목록")]
    [SerializeField] private List<DialogueEvent> dialogueEvents;

    private bool inRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) inRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) inRange = false;
    }

    private void Update()
    {
        if (!inRange || runner == null || graph == null) return;

        if (Input.GetKeyDown(interactKey) && !runner.IsRunning)
        {
            // ▼ Run 함수를 호출할 때, "이벤트 처리 함수(HandleDialogueEvent)"를 같이 넘겨줍니다.
            runner.Run(graph, HandleDialogueEvent);
        }
    }

    // 러너가 "OpenUpgrade" 같은 키를 보내면 여기서 받아서 처리
    private void HandleDialogueEvent(string key)
    {
        foreach (var evt in dialogueEvents)
        {
            if (evt.eventKey == key)
            {
                evt.action.Invoke(); // 연결된 UnityEvent 실행!
                return;
            }
        }
        Debug.LogWarning($"이벤트 키 '{key}'에 해당하는 동작이 {gameObject.name}에 정의되지 않았습니다.");
    }
}