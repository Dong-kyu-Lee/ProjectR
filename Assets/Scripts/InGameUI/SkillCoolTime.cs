using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField]
    public GameObject hideImg;

    private bool canUseSkill = true;
    private float coolTime = 5f;
    private float currentTime = 0f;

    private Image imageComponent;

    private void Start()
    {
        imageComponent = hideImg.GetComponent<Image>();
        hideImg.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canUseSkill)
        {
            canUseSkill = false;
            currentTime = coolTime;
            hideImg.SetActive(true);
            imageComponent.fillAmount = 1f;

            StartCoroutine(SkillCooldownRoutine());
        }
    }

    IEnumerator SkillCooldownRoutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            imageComponent.fillAmount = currentTime / coolTime;

            yield return null;
        }
        imageComponent.fillAmount = 0f;
        hideImg.SetActive(false);
        canUseSkill = true;
    }
}