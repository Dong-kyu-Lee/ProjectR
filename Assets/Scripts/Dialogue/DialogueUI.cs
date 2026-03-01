using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("패널 및 텍스트")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private Text nameText;     // 이름 텍스트 (NameText)
    [SerializeField] private Text dialogueText; // 대사 텍스트 (NpcSaying)

    [Header("선택지 버튼 (연결 필요)")]
    [SerializeField] private GameObject yesButtonObj;      // YesButton 오브젝트
    [SerializeField] private GameObject noButtonObj;       // NoButton 오브젝트
    [SerializeField] private Text yesText;      // Yes 버튼 내부 텍스트
    [SerializeField] private Text noText;       // No 버튼 내부 텍스트

    [Header("설정")]
    [SerializeField] private float typingSpeed = 0.05f;

    private bool isTyping;
    private Coroutine typingCoroutine;
    private string currentFullText;

    private void Start()
    {
        Close();
    }
    // 초기화
    public void Open()
    {
        rootPanel.SetActive(true);
        yesButtonObj.SetActive(false); // 버튼 숨김
        noButtonObj.SetActive(false);
        nameText.text = "";
        dialogueText.text = "";
    }

    public void Close()
    {
        rootPanel.SetActive(false);
    }

    // 일반 대화(SayNode) 출력
    public void SetSayNode(SayNode node)
    {
        // 선택지 버튼들은 숨김
        yesButtonObj.SetActive(false);
        noButtonObj.SetActive(false);

        // 이름 설정
        if (node.speaker != null)
        {
            nameText.text = node.speaker.characterName;
        }
        else
        {
            nameText.text = "???"; // 화자가 없으면 ???로 표기
        }

        // 타이핑 시작
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypewriterRoutine(node.text));
    }

    // 선택지(ChoiceNode) 출력
    public void SetChoiceNode(ChoiceNode node, System.Action onYes, System.Action onNo)
    {
        // 타이핑 중단하고 질문 텍스트 즉시 완성
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (node.speaker != null) nameText.text = node.speaker.characterName;
        else nameText.text = "";

        dialogueText.text = node.question;

        // 버튼 활성화
        yesButtonObj.SetActive(true);
        noButtonObj.SetActive(true);
        yesText.text = node.yesText;
        noText.text = node.noText;

        // 버튼 이벤트 연결
        Button yBtn = yesButtonObj.GetComponent<Button>();
        Button nBtn = noButtonObj.GetComponent<Button>();

        yBtn.onClick.RemoveAllListeners();
        nBtn.onClick.RemoveAllListeners();

        yBtn.onClick.AddListener(() => onYes.Invoke());
        nBtn.onClick.AddListener(() => onNo.Invoke());
    }

    // 타이핑 효과 코루틴
    private IEnumerator TypewriterRoutine(string text)
    {
        isTyping = true;
        currentFullText = text;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    // 클릭 시 타이핑 스킵
    public bool TrySkipTyping()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentFullText;
            isTyping = false;
            return true;
        }
        return false;
    }
}