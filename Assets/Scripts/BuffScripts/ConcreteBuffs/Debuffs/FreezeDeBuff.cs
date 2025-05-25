using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class FreezeDeBuff : Buff
{
    private float freezeBustDmg = 0.05f;
    private float moveSpeedDec = 0.5f;

    public FreezeDeBuff(float duration, GameObject target) : base(duration, target) 
    {
        this.BuffType = BuffType.Freeze;
        maxDuration = 8;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;
        CalcReceiveDamage.Instance.TakeDebuffDamage(targetStatus.MaxHp * freezeBustDmg, targetStatus);
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(targetStatus.MaxHp * freezeBustDmg, targetStatus);
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<Status>().AdditionalMoveSpeed += moveSpeedDec;
    }
}
