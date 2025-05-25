using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDeBuff : Buff
{
    private float[] poisonDmg = { 0.01f, 0.02f, 0.03f }; // 레벨별 틱당 독 데미지

    public PoisonDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Poison;
        maxDuration = 10;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(targetStatus.MaxHp * poisonDmg[currentBuffLevel], targetStatus);
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        base.DoActionOnActivate(tickDuration);
        ApplyBuffEffect();
    }

    public override void RemoveBuffEffect()
    {
    }
}
