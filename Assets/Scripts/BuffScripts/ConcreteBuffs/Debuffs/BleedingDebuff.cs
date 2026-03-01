using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingDebuff : Buff
{
    private float[] bleedingDmg = { 0.2f, 0.4f, 0.6f, 0.8f, 1.0f }; // 레벨별 틱당 출혈 데미지
    private float tickTimer = 0f; // 틱 타이머

    public BleedingDebuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Bleeding;
        maxBuffLevel = 5;
        maxDuration = 3f;
        buffEffectTick = 0.5f;
        isDebuff = true;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(bleedingDmg[currentBuffLevel], targetStatus, true);

        if (InGameUIManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            PlayerStatus status = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            if (status != null)
            {
                InGameUIManager.Instance.UpdateHpSmooth(status.Hp, status.MaxHp);
            }
        }

    }

    public override void DoActionOnActivate(float deltaTime)
    {
        tickTimer += deltaTime;

        if (tickTimer >= 1f)
        {
            ApplyBuffEffect();
            tickTimer = 0f;
        }

        base.DoActionOnActivate(deltaTime); // 지속시간 감소
    }

    public override void RemoveBuffEffect()
    {
    }
}
