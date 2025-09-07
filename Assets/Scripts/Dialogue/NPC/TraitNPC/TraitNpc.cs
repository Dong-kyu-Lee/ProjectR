using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TraitNpc : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private ConversationUIController conversationUI;
    [SerializeField] private GameObject traitWindow; // 특성 창 루트

    [Header("설정")]
    [TextArea]
    [SerializeField]
    private string prompt =
        "또 왔구만.\n이번에도 새로운 가능성을 열어볼텐가?";
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    private bool playerInRange;
    private bool isInteracting;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // 트리거로 사용
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            isInteracting = false;
        }
    }

    private void Update()
    {
        if (!playerInRange || isInteracting) return;

        if (Input.GetKeyDown(interactKey))
        {
            isInteracting = true;
            conversationUI.Open(prompt, OnYes, OnNo);
        }
    }

    private void OnYes()
    {
        if (traitWindow) traitWindow.SetActive(true); // 특성 창 열기
        isInteracting = false;
    }

    private void OnNo()
    {
        conversationUI.OpenSingle(
            "그래. 다음에 또 오게.\n잘 가게나",
            ok: () => { isInteracting = false; },   // 확인 누르면 종료
            okText: "알겠어"
        );
    }

}
