using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenDeBuff : Buff
{
    private float DrunkenBustDmg = 20f;
    private float moveSpeedDec = 10.0f;

    public DrunkenDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Drunken;
        maxDuration = 2;
        maxBuffLevel = 1;
        isDebuff = true;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;
        
        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;
        if (AbilityManager.Instance.bartenderAbility[2]) CalcReceiveDamage.Instance.TakeDebuffDamage(DrunkenBustDmg, targetStatus, false);
        BuffEffectManager.Instance.PlayBuffEffect(this.BuffType, targetObject.transform.position, false);
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (AbilityManager.Instance.bartenderAbility[2]) CalcReceiveDamage.Instance.TakeDebuffDamage(DrunkenBustDmg, targetStatus, false);
        BuffEffectManager.Instance.PlayBuffEffect(this.BuffType, targetObject.transform.position, false);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed += moveSpeedDec;
    }
}
