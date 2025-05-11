using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class BlessBuff : Buff
{
    public override BuffType BuffType => BuffType.Bless;
    private float[] healAmount = { 1.0f, 2.0f, 3.0f };    //틱당 체력 회복 량

    public BlessBuff(float duration, GameObject target) : base(duration, target) { }

    //대상에게 특정 스탯 증가량을 적용시키는 메서드. 하는 일 없음.
    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.Hp += healAmount[currentBuffLevel];
    }

    //대상에게 도트힐을 제공하는 메서드. 해당 버프는 ApplyBuffEffect()를 대신해 기능함
    public override void DoActionOnActivate(float tickDuration = 1.0f)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    //대상에게 적용된 스탯 증가량을 해제 메서드. 하는 일 없음.
    public override void RemoveBuffEffect()
    { }
}
