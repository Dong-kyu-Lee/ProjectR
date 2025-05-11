using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Slow; 
    private float[] moveSpeedDecGap = { 30.0f };    //이동속도 감소량. 슬로우 디버프에 레벨이 더 생기면 여기에 값을 추가
    private float speedDecAmount = 0.0f;            //실질적으로 감소된 이동속도의 양
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