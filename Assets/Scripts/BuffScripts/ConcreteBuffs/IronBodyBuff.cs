using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBodyBuff : Buff
{
    private float[] damageReduceIncGap = { 0.2f, 0.25f, 0.3f };  //피해감소량 증가 간격
    private float[] attackSpeedIncGap = { -0.2f, 0.1f, 0.1f };  //공격속도 증가 간격
    private float[] moveSpeedIncGap = { -0.1f, 0.05f, 0.05f };  //이동속도 증가 간격


    public IronBodyBuff(float totalDuration, GameObject target) : base(totalDuration, target) {
        this.BuffType = BuffType.IronBody;
        if (CalcDamage.Instance.mysteryEffect13) maxBuffLevel = 3;
        else maxBuffLevel = 2;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증감하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        if (currentBuffLevel > 0)
        {
            targetStatus.AdditionalDamageReduction -= damageReduceIncGap[currentBuffLevel - 1]; ;
            targetStatus.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];
        }
        else targetStatus.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];

        targetStatus.AdditionalAttackSpeed += attackSpeedIncGap[currentBuffLevel];
        targetStatus.AdditionalMoveSpeed += moveSpeedIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 증감하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamageReduction -= damageReduceIncGap[currentBuffLevel];
        targetStatus.AdditionalAttackSpeed -= GetCurrentSumOfArray(attackSpeedIncGap);
        targetStatus.MoveSpeed -= GetCurrentSumOfArray(moveSpeedIncGap);
    }
}
