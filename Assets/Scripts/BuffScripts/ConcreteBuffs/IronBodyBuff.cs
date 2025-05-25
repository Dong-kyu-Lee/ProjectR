using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBodyBuff : Buff
{
    private float[] damageReduceGap = { 0.1f, 0.2f, 0.2f };  //피해감소량 증가량 간격
    private float[] attackSpeedDecGap = { 0.1f, 0.2f, 0.3f };  //공격속도 감소량 증가량 간격
    private float[] moveSpeedDecGap = { 0.02f, 0.03f, 0.05f };  //이동속도 감소량 증가량 간격


    public IronBodyBuff(float totalDuration, GameObject target) : base(totalDuration, target) {
        this.BuffType = BuffType.IronBody;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증감하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamageReduction += damageReduceGap[currentBuffLevel];
        targetStatus.AdditionalAttackSpeed -= attackSpeedDecGap[currentBuffLevel];
        targetStatus.AdditionalMoveSpeed -= moveSpeedDecGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 증감하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamageReduction -= GetCurrentSumOfArray(damageReduceGap);
        targetStatus.AdditionalAttackSpeed += GetCurrentSumOfArray(attackSpeedDecGap);
        targetStatus.MoveSpeed += GetCurrentSumOfArray(moveSpeedDecGap);
    }
}
