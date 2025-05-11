using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBodyBuff : Buff
{
    public override BuffType BuffType => BuffType.IronBody;  
    private float[] damageReduceGap = { 10.0f, 20.0f, 20.0f };  //피해감소량 증가량 간격
    private float[] attackSpeedDecGap = { 10.0f, 20.0f, 30.0f };  //공격속도 감소량 증가량 간격
    private float[] moveSpeedDecGap = { 2.0f, 3.0f, 5.0f };  //이동속도 감소량 증가량 간격


    public IronBodyBuff(float totalDuration, GameObject target) : base(totalDuration, target) { }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증감하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.AdditionalDamageReduction += damageReduceGap[currentBuffLevel];
        playerStatus.AdditionalAttackSpeed -= attackSpeedDecGap[currentBuffLevel];
        playerStatus.MoveSpeed -= moveSpeedDecGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 증감하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.AdditionalDamageReduction -= GetCurrentSumOfArray(damageReduceGap);
        playerStatus.AdditionalAttackSpeed += GetCurrentSumOfArray(attackSpeedDecGap);
        playerStatus.MoveSpeed += GetCurrentSumOfArray(moveSpeedDecGap);
    }
}
