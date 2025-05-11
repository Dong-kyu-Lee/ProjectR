using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnDeBuff : Buff
{
    private float fireBustDmg = 0.05f; // 화염 버스트 데미지
    private float fireTickDmg = 0.01f; // 틱당 화염 데미지

    public BurnDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxDuration = 5;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        CalcReceiveDamage.Instance.TakeDebuffDamageQueue(targetStatus.MaxHp * fireBustDmg, targetStatus.gameObject);
        targetStatus.Hp -= targetStatus.MaxHp * fireBustDmg;
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        CalcReceiveDamage.Instance.TakeDebuffDamageQueue(targetStatus.MaxHp * fireBustDmg, targetStatus.gameObject);
        targetStatus.Hp -= targetStatus.MaxHp * fireBustDmg;
    }

    private void TickDamage()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        CalcReceiveDamage.Instance.TakeDebuffDamageQueue(targetStatus.MaxHp * fireTickDmg, targetStatus.gameObject);
        targetStatus.Hp -= targetStatus.MaxHp * fireTickDmg;
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
