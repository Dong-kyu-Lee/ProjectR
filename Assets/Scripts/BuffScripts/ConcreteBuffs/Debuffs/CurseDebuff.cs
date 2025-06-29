using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseDeBuff : Buff
{
    private float moveSpeedDec = 10.0f;
    private float curseBustPerDmg = 0.05f;

    public CurseDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Curse;
        maxDuration = 15f;
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;
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

        targetStatus.AdditionalMoveSpeed += moveSpeedDec;
    }
}