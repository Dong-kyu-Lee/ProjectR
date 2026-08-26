using System.Collections;
using UnityEngine;
using TMPro;

public class LevelUpText : MonoBehaviour
{
    private TMP_Text tmpText;
    private Vector3 initialScale;     // 시작 크기
    private Color initialColor;       // 기존 설정 색상

    [Header("강조 연출 설정")]
    [SerializeField] private float punchScaleAmount = 1.3f; // 팅~ 하고 커질 최대 크기 비율
    [SerializeField] private float floatUpHeight = 0.8f;    // 떠오를 높이

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        initialScale = transform.localScale;
        initialColor = tmpText.color;

        // 시작할 때는 숨김
        gameObject.SetActive(false);
    }

    public void PlayLevelUpText(float duration = 1.5f)
    {
        Color startColor = initialColor;
        transform.localScale = initialScale;
        startColor.a = 1f; // 알파값 불투명으로 리셋
        tmpText.color = startColor;

        gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(AnimateRoutine(duration));
    }

    private IEnumerator AnimateRoutine(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 1. 크기 강조 연출 (시작할 때 1.3배로 커졌다가 부드럽게 원래 크기로 돌아옴)
            if (progress < 0.2f)
            {
                // 앞의 20% 시간 동안 팅! 하고 커짐
                float scaleProgress = progress / 0.2f;
                transform.localScale = Vector3.Lerp(initialScale, initialScale * punchScaleAmount, scaleProgress);
            }

            // 2. 투명도 감소 (서서히 페이드아웃)
            Color color = tmpText.color;
            color.a = Mathf.Clamp01((1f - progress) * 2f);
            tmpText.color = color;

            yield return null;
        }

        // 완전히 사라진 후 비활성화
        gameObject.SetActive(false);
    }
}
