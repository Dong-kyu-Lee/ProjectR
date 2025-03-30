using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeDeBuff : Buff
{
    private float freezeBustDmg = 0.05f;
    private float moveSpeedDec = 0.5f;

    public FreezeDeBuff(float duration, GameObject target) : base(duration, target) 
    {
        maxDuration = 8;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;
        CalcReceiveDamage.Instance.TakeTrueDamageQueue(targetStatus.MaxHp * freezeBustDmg, targetStatus.gameObject);
        targetStatus.Hp -= targetStatus.MaxHp * freezeBustDmg;
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        CalcReceiveDamage.Instance.TakeTrueDamageQueue(targetStatus.MaxHp * freezeBustDmg, targetStatus.gameObject);
        targetStatus.Hp -= targetStatus.MaxHp * freezeBustDmg;
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<Status>().AdditionalMoveSpeed += moveSpeedDec;
    }
}
