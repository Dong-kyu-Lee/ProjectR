using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ConversationUIController : MonoBehaviour
{
    private enum UIState { Closed, Opening, Open, Closing }

    [Header("Refs")]
    [SerializeField] private Text messageText;

    [SerializeField] private Button yesButton;
    [SerializeField] private Text yesButtonLabel;   // YesButton

    [SerializeField] private Button noButton;
    [SerializeField] private Text noButtonLabel;    // NoButton

    [Header("Options")]
    [SerializeField] private string defaultYesText = "좋다.";
    [SerializeField] private string defaultNoText = "싫다.";
    [SerializeField] private float fadeDuration = 0.12f;

    //중복 오픈 방지 옵션
    [Tooltip("창이 이미 열려있을 때(Opening/Open 상태) 추가 Open 호출을 무시합니다.")]
    [SerializeField] private bool ignoreIfAlreadyOpen = true;

    [Tooltip("무시 대신, 열려있는 상태에서 메시지/버튼 텍스트와 콜백만 갱신합니다.")]
    [SerializeField] private bool refreshTextIfReopen = false;

    private CanvasGroup group;
    private Action onYes;
    private Action onNo;

    private UIState state = UIState.Closed;

    public bool IsOpen => group && state == UIState.Open;

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

    private void OnDisable()
    {
        // 비활성화/씬 전환 중 코루틴 정리
        StopAllCoroutines();
        state = UIState.Closed;
        if (group)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
        }
        onYes = null; onNo = null;
    }

    // 2버튼 모드
    public void OpenChoices(string message, Action yes = null, Action no = null,
                            string yesText = null, string noText = null)
    {
        //중복 오픈 방지
        if (state == UIState.Opening || state == UIState.Open)
        {
            if (ignoreIfAlreadyOpen && !refreshTextIfReopen)
                return;

            // 열려있는 상태에서 새 메시지/버튼/콜백만 갱신
            onYes = yes; onNo = no;
            if (messageText) messageText.text = message ?? messageText?.text;

            SetButtonVisibility(true, true);
            if (yesButtonLabel) yesButtonLabel.text = string.IsNullOrEmpty(yesText) ? defaultYesText : yesText;
            if (noButtonLabel) noButtonLabel.text = string.IsNullOrEmpty(noText) ? defaultNoText : noText;

            return;
        }

        onYes = yes; onNo = no;
        if (messageText) messageText.text = message ?? "";

        SetButtonVisibility(true, true);
        if (yesButtonLabel) yesButtonLabel.text = string.IsNullOrEmpty(yesText) ? defaultYesText : yesText;
        if (noButtonLabel) noButtonLabel.text = string.IsNullOrEmpty(noText) ? defaultNoText : noText;

        Show();
    }

    public void Open(string message, Action yes = null, Action no = null)
    {
        OpenChoices(message, yes, no, null, null);
    }

    public void OpenSingle(string message, Action ok = null, string okText = "알겠어")
    {
        if (state == UIState.Opening || state == UIState.Open)
        {
            if (ignoreIfAlreadyOpen && !refreshTextIfReopen)
                return;

            onYes = ok; onNo = null;
            if (messageText) messageText.text = message ?? messageText?.text;

            SetButtonVisibility(true, false);
            if (yesButtonLabel) yesButtonLabel.text = okText;
            return;
        }

        onYes = ok; onNo = null;
        if (messageText) messageText.text = message ?? "";

        SetButtonVisibility(true, false);
        if (yesButtonLabel) yesButtonLabel.text = okText;

        Show();
    }

    public void Close()
    {
        state = UIState.Closing;
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
        state = UIState.Opening;
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

        state = target > 0f ? UIState.Open : UIState.Closed;
    }
}
