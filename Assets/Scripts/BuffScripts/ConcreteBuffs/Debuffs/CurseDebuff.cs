using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseDeBuff : Buff
{
    private float damageTakenInc = 0.03f;
    private float curseBustPerDmg = 0.05f;

    public CurseDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Curse;
        maxDuration = 10;
        maxBuffLevel = 5;
        if (CalcDamage.Instance.curseEffect13) damageTakenInc = 0.08f;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.DamageTaken += damageTakenInc;
        if (CalcDamage.Instance.curseEffect16) targetStatus.TakeTrueDamage(curseBustPerDmg * targetStatus.MaxHp);
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (CalcDamage.Instance.curseEffect16) targetStatus.TakeTrueDamage(curseBustPerDmg * targetStatus.MaxHp);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.DamageTaken -= damageTakenInc * (currentBuffLevel + 1);
    }
}