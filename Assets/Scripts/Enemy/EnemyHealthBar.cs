using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyStatus enemyStatus;
    [SerializeField] private Image hpFill;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        enemyStatus.OnHpChanged += UpdateHpBar;
    }

    private void OnDisable()
    {
        enemyStatus.OnHpChanged -= UpdateHpBar;
    }

    private void UpdateHpBar(float current, float max)
    {
        hpFill.fillAmount = current / max;

        if (hpFill.fillAmount < 1f)
            canvasGroup.alpha = 1f;
    }

    void LateUpdate()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
