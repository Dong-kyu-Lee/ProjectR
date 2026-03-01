using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField] public GameObject hideImg;
    [SerializeField] private Image originalIcon;

    private Image cooldownImageComponent;
    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        if (hideImg != null)
        {
            cooldownImageComponent = hideImg.GetComponent<Image>();
            hideImg.SetActive(false);
        }
    }

    public void SetSkillIcon(Sprite newIcon)
    {
        if (newIcon == null) return;

        // 밝은 아이콘 교체
        if (originalIcon != null)
        {
            originalIcon.sprite = newIcon;
        }

        // 어두운 쿨타임 배경 아이콘 교체
        if (cooldownImageComponent != null)
        {
            cooldownImageComponent.sprite = newIcon;
        }
    }

    // 외부에서 이 함수를 부르면 시작
    public void TriggerCooldown(float time)
    {
        if (hideImg == null) return;

        // 이미 돌고 있다면 멈추고 새로 시작
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);

        hideImg.SetActive(true);
        if (cooldownImageComponent != null) cooldownImageComponent.fillAmount = 1f;

        cooldownCoroutine = StartCoroutine(CooldownRoutine(time));
    }

    // 쿨타임 UI 강제 초기화 (캐릭터 교체 시 사용)
    public void ResetCooldownUI()
    {
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        if (hideImg != null) hideImg.SetActive(false);
        if (cooldownImageComponent != null) cooldownImageComponent.fillAmount = 0f;
    }

    // 실질적으로 그림을 그리는 코루틴
    IEnumerator CooldownRoutine(float time)
    {
        float currentTime = time;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (cooldownImageComponent != null)
                cooldownImageComponent.fillAmount = currentTime / time;
            yield return null;
        }

        if (hideImg != null) hideImg.SetActive(false);
        if (cooldownImageComponent != null) cooldownImageComponent.fillAmount = 0f;
    }
}