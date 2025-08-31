using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ConversationUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Text messageText;

    [SerializeField] private Button yesButton;
    [SerializeField] private Text yesButtonLabel;   // YesButton/Yes(Text)

    [SerializeField] private Button noButton;
    [SerializeField] private Text noButtonLabel;    // NoButton/No(Text)

    [Header("Options")]
    [SerializeField] private string defaultYesText = "좋다.";
    [SerializeField] private string defaultNoText = "싫다.";
    [SerializeField] private float fadeDuration = 0.12f;

    private CanvasGroup group;
    private Action onYes;
    private Action onNo;

    public bool IsOpen => group && group.alpha > 0.01f;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
        }
        if (yesButton) yesButton.onClick.AddListener(OnClickYes);
        if (noButton) noButton.onClick.AddListener(OnClickNo);
    }

    // 2버튼 모드
    public void OpenChoices(string message, Action yes = null, Action no = null,
                            string yesText = null, string noText = null)
    {
        onYes = yes; onNo = no;
        if (messageText) messageText.text = message ?? "";

        SetButtonVisibility(true, true);
        if (yesButtonLabel) yesButtonLabel.text = string.IsNullOrEmpty(yesText) ? defaultYesText : yesText;
        if (noButtonLabel) noButtonLabel.text = string.IsNullOrEmpty(noText) ? defaultNoText : noText;

        Show();
    }
    public void Open(string message, Action yes = null, Action no = null)
    {
        OpenChoices(message, yes, no, null, null); // 기존 코드와 동일 동작
    }
    // 1버튼 모드 (알겠어)
    public void OpenSingle(string message, Action ok = null, string okText = "알겠어")
    {
        onYes = ok; onNo = null;
        if (messageText) messageText.text = message ?? "";

        SetButtonVisibility(true, false);
        if (yesButtonLabel) yesButtonLabel.text = okText;

        Show();
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(0f));
        onYes = null; onNo = null;
    }

    private void OnClickYes()
    {
        var cb = onYes;
        Close();
        cb?.Invoke();
    }
    private void OnClickNo()
    {
        var cb = onNo;
        Close();
        cb?.Invoke();
    }

    private void SetButtonVisibility(bool yesOn, bool noOn)
    {
        if (yesButton) yesButton.gameObject.SetActive(yesOn);
        if (noButton) noButton.gameObject.SetActive(noOn);
    }

    private void Show()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(1f));
    }

    private System.Collections.IEnumerator FadeTo(float target)
    {
        if (!group) yield break;
        group.blocksRaycasts = target > 0f;
        group.interactable = target > 0f;

        float start = group.alpha, t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        group.alpha = target;
    }
}
