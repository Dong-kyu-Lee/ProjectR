using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class BlessBuff : Buff
{
    private float[] healAmount = { 1.0f, 2.0f, 3.0f };    //틱(0.1초)당 체력 회복 량

    public BlessBuff(float duration, GameObject target) : base(duration, target) { }

    //대상에게 도트힐을 제공하는 메서드. 해당 버프는 ApplyBuffEffect()를 대신해 기능함
    public override void DoActionOnActivate(float tickDuration = 1.0f)
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.Hp += healAmount[currentBuffLevel];
        Debug.Log($"플레이어 체력 : {playerStatus.Hp}");
        CurrentDuration -= tickDuration;
    }

    //대상에게 특정 스탯 증가량을 적용시키는 메서드. 하는 일 없음.
    public override void ApplyBuffEffect()
    {
    }

    //대상에게 적용된 스탯 증가량을 해제 메서드. 하는 일 없음.
    public override void RemoveBuffEffect()
    {
    }
}
