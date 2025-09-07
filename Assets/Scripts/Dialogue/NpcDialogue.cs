using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NpcDialogue : MonoBehaviour
{
    //NPC 상호작용 트리거. 범위 내에서 E 누르면 러너가 그래프를 실행.
    [Header("Refs")]
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private DialogueGraph graph;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    private bool inRange;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

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
            runner.Run(graph);
        }
    }
}
