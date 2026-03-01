using System.Collections;
using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private SpriteRenderer background;

    [Header("Lines")]
    [TextArea]
    public string[] lines = {    };
    [SerializeField] private bool randomOrder = true;

    [Header("Timing (sec)")]
    [SerializeField] private Vector2 intervalRange = new Vector2(6f, 12f); // 다음 멘트까지 대기
    [SerializeField] private float showDuration = 2.5f;                    // 노출 시간
    [SerializeField] private float fadeDuration = 0.12f;                   // 페이드 시간

    int seqIndex;
    Coroutine loopCo;

    void OnEnable()
    {
        SetAlpha(0f);
        if (loopCo != null) StopCoroutine(loopCo);
        loopCo = StartCoroutine(BarkLoop());
    }

    void OnDisable()
    {
        if (loopCo != null) StopCoroutine(loopCo);
        SetAlpha(0f);
    }

    IEnumerator BarkLoop()
    {
        if (lines == null || lines.Length == 0 || text == null) yield break;

        while (true)
        {
            // 라인 선택
            string msg = randomOrder ? lines[Random.Range(0, lines.Length)] : lines[seqIndex];
            if (!randomOrder) seqIndex = (seqIndex + 1) % lines.Length;

            // 표시
            text.text = msg;
            yield return FadeTo(1f);
            yield return new WaitForSeconds(showDuration);
            yield return FadeTo(0f);

            // 대기
            float wait = Random.Range(intervalRange.x, intervalRange.y);
            yield return new WaitForSeconds(wait);
        }
    }

    void SetAlpha(float a)
    {
        if (text) { var c = text.color; c.a = a; text.color = c; }
        if (background) { var c = background.color; c.a = a; background.color = c; }
    }

    IEnumerator FadeTo(float target)
    {
        float start = text ? text.color.a : 0f;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, target, t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(target);
    }
}
