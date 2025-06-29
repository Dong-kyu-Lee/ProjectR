using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnDeBuff : Buff
{
    private float fireBustDmg = 10f; // 화염 버스트 데미지
    private float fireTickDmg = 2f; // 틱당 화염 데미지

    public BurnDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Burn;
        maxDuration = 5;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(fireBustDmg, targetStatus, false);
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(fireBustDmg, targetStatus, false);
    }

    private void TickDamage()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(fireTickDmg, targetStatus, false);
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        base.DoActionOnActivate(tickDuration);
        TickDamage();
    }

    public override void RemoveBuffEffect()
    {
    }
}
