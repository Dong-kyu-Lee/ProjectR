using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField] public GameObject hideImg;

    private Image imageComponent;
    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        if (hideImg != null)
        {
            imageComponent = hideImg.GetComponent<Image>();
            hideImg.SetActive(false);
        }
    }

    // 외부에서 이 함수를 부르면 시작
    public void TriggerCooldown(float time)
    {
        if (hideImg == null) return;

        // 이미 돌고 있다면 멈추고 새로 시작
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);

        hideImg.SetActive(true);
        if (imageComponent != null) imageComponent.fillAmount = 1f;

        cooldownCoroutine = StartCoroutine(CooldownRoutine(time));
    }

    // 쿨타임 UI 강제 초기화 (캐릭터 교체 시 사용)
    public void ResetCooldownUI()
    {
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        if (hideImg != null) hideImg.SetActive(false);
        if (imageComponent != null) imageComponent.fillAmount = 0f;
    }

    // 실질적으로 그림을 그리는 코루틴
    IEnumerator CooldownRoutine(float time)
    {
        float currentTime = time;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (imageComponent != null)
                imageComponent.fillAmount = currentTime / time;
            yield return null;
        }

        if (hideImg != null) hideImg.SetActive(false);
        if (imageComponent != null) imageComponent.fillAmount = 0f;
    }
}