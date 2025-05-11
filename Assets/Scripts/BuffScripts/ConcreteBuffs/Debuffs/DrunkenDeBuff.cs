using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenDeBuff : Buff
{
    private float DrunkenBustDmg = 0.1f;

    public DrunkenDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Drunken;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.AdditionalMoveSpeed -= 10.0f;
        if (AbilityManager.Instance.bartenderAbility3)
        {
            CalcReceiveDamage.Instance.TakeDebuffDamageQueue(targetStatus.MaxHp * DrunkenBustDmg, targetStatus.gameObject);
            targetStatus.Hp -= targetStatus.MaxHp * DrunkenBustDmg;
        }
        Debug.Log("만취 디버프 부여");
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.AdditionalMoveSpeed += 10.0f;
        Debug.Log("만취 디버프 해제");
    }
}
