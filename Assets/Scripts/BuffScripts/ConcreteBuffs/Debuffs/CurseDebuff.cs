using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseDeBuff : Buff
{
    private float damageTakenInc = 0.03f;

    public CurseDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Curse;
        maxDuration = 10;
        maxBuffLevel = 5;
        if (CalcDamage.Instance.curseEffect13) damageTakenInc = 0.1f;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.DamageTaken += damageTakenInc;
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.DamageTaken -= damageTakenInc * (currentBuffLevel + 1);
    }
}