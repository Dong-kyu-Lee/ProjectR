using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingDebuff : Buff
{
    private float[] bleedingDmg = { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f }; // 레벨별 틱당 출혈 데미지
    private float tickTimer = 0f; // 틱 타이머

    public BleedingDebuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Bleeding;
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(bleedingDmg[currentBuffLevel], targetStatus);

        if (InGameUIManager.Instance != null)
            InGameUIManager.Instance.CheckHp();
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
