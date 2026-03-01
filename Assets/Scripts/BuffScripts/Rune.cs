using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Rune : MonoBehaviour
{
    public BuffType buffType;
    private SpriteRenderer spriteRenderer;
    private bool isBuffActive = false;

    void Start()
    {
        buffType = RuneSpawner.Instance.GetRandomBuffType();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetColorForBuff(buffType);
        }
    }

    private Color GetColorForBuff(BuffType type)
    {
        switch (type)
        {
            case BuffType.AttackDamageIncrease:
                return new Color(1f, 0.5f, 0f);
            case BuffType.DamageReductionIncrease:
                return new Color(0.6f, 1f, 0.4f);
            case BuffType.CritPercentIncrease:
                return Color.yellow;
            case BuffType.CritDamageIncrease:
                return Color.red;
            case BuffType.AttackSpeedIncrease:
                return Color.cyan;
            case BuffType.MoveSpeedIncrease:
                return Color.white;
            case BuffType.BulkUp:
                return new Color(0.8f, 0.4f, 0.2f);
            case BuffType.EagleEye:
                return Color.blue;
            case BuffType.ExtremeSpeed:
                return Color.green;
            case BuffType.IronBody:
                return new Color(0.5f, 0.5f, 0.5f);
            case BuffType.Raging:
                return new Color(0.9f, 0f, 0f);
            default:
                return Color.gray;
        }
    }

    // 플레이어와 닿았을 때 버프 활성화.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBuffActive)
        {
            BuffManager playerBuffManager = other.GetComponent<BuffManager>();
            if (playerBuffManager != null)
            {
                playerBuffManager.ActivateBuff(buffType, 60f);
                if (CalcDamage.Instance.mysteryEffect16) playerBuffManager.ActivateBuff(BuffType.Destruction, 10f);
                isBuffActive = true;
            }

            Destroy(gameObject);
        }
    }
}