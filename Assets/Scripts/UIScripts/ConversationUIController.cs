using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ConversationUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Text messageText;       // 질문/대사 텍스트
    [SerializeField] private Button yesButton;       // YesButton 오브젝트의 Button
    [SerializeField] private Button noButton;        // NoButton 오브젝트의 Button
    [SerializeField] private float fadeDuration = 0.12f;

    private CanvasGroup group;
    private Action onYes;
    private Action onNo;

    public bool IsOpen => group && group.alpha > 0.01f;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
        }

        if (yesButton) yesButton.onClick.AddListener(OnClickYes);
        if (noButton) noButton.onClick.AddListener(OnClickNo);
    }

    public void Open(string message, Action yes = null, Action no = null)
    {
        onYes = yes;
        onNo = no;

        if (messageText) messageText.text = message ?? "";

        StopAllCoroutines();
        StartCoroutine(FadeTo(1f));
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(0f));
        onYes = null;
        onNo = null;
    }

    public void OnClickYes()
    {
        var cb = onYes; // 콜백 실행 전에 닫기
        Close();
        cb?.Invoke();
    }

    public void OnClickNo()
    {
        var cb = onNo;
        Close();
        cb?.Invoke();
    }

    private System.Collections.IEnumerator FadeTo(float target)
    {
        if (!group) yield break;

        group.blocksRaycasts = target > 0f;
        group.interactable = target > 0f;

        float start = group.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        group.alpha = target;
    }
}
