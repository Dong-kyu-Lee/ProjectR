using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedIncBuff : Buff
{
    private float[] moveSpeedIncGap = { 10.0f, 40.0f, 50.0f };  //이동속도 증가량 간격
    public MoveSpeedIncBuff(float duration, GameObject target) : base(duration, target) { }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        targetObject.GetComponent<Status>().MoveSpeed += moveSpeedIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 증감하는 식
    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().MoveSpeed -= GetCurrentSumOfArray(moveSpeedIncGap);
    }
}
