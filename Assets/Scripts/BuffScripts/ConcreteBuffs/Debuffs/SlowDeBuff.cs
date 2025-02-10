using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDeBuff : Buff
{
    private float[] moveSpeedDecGap = { 30.0f };    //이동속도 감소 30%
    private float speedDecAmount = 0.0f;            //실질적 이동속도 감소량
    public SlowDeBuff(float duration, GameObject target) : base(duration, target) 
    {
        maxBuffLevel = 1;   //단일치 버프
    }

    public override void ApplyBuffEffect()
    {
        PlayerStatus temp = targetObject.GetComponent<PlayerStatus>();
        speedDecAmount = temp.MoveSpeed * moveSpeedDecGap[currentBuffLevel] * 0.01f;
        temp.MoveSpeed -= speedDecAmount;
    }

    public override void RemoveBuffEffect()
    {
        PlayerStatus temp = targetObject.GetComponent<PlayerStatus>();
        temp.MoveSpeed += speedDecAmount;
    }
}