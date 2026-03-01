using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TextMeshPro damageText;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float duration;
    private float elapsedTime;
    private bool active;

    private void Awake()
    {
        damageText = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        Color color = damageText.color;
        color.a = 1f;
        damageText.color = color;
        active = false;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > 0.1f && !active)
        {
            ObjectPoolManager.Instance.activeDamageTexts--;
            active = true;
        }

        transform.Translate(new Vector3(0, Time.deltaTime * moveSpeed, 0));

        // 지정된 시간이 지나면 반환
        if (elapsedTime >= duration)
        {
            Color color = damageText.color;
            color.a = Mathf.Lerp(color.a, 0f, fadeSpeed);
            damageText.color = color;
            if (elapsedTime >= duration + 0.5f)
                ObjectPoolManager.Instance.ReturnDamageText(gameObject);
        }
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }
}
