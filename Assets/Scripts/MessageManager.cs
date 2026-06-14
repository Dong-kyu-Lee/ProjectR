using System.Collections;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    private Coroutine fadeCoroutine;

    // 메시지 띄우기
    public void ShowMessage(string message, float delay = 3.0f)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(DisplayMessageCo(message, delay));
    }

    // 메시지 띄우기 코루틴
    private IEnumerator DisplayMessageCo(string message, float delay)
    {
        statusText.text = message;
        Color originalColor = statusText.color;
        originalColor.a = 1f;
        statusText.color = originalColor;

        statusText.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            if (statusText == null) yield break;

            elapsedTime += Time.deltaTime;

            originalColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            statusText.color = originalColor;

            yield return null;
        }

        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }
    }
}
