using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenDeBuff : Buff
{
    private float DrunkenBustDmg = 0.1f;
    private float moveSpeedDec = 10.0f;

    public DrunkenDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Drunken;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;
        
        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;
        if (AbilityManager.Instance.bartenderAbility3) CalcReceiveDamage.Instance.TakeDebuffDamage(targetStatus.MaxHp * DrunkenBustDmg, targetStatus);
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (AbilityManager.Instance.bartenderAbility3) CalcReceiveDamage.Instance.TakeDebuffDamage(targetStatus.MaxHp * DrunkenBustDmg, targetStatus);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed += moveSpeedDec;
    }
}
