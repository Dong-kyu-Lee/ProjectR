using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedIncBuff : Buff
{
    private float[] attackSpeedIncGap = { 0.1f, 0.2f, 0.5f }; //공격속도 증가량 간격

    public AttackSpeedIncBuff(float duration, GameObject target) : base(duration, target) {}

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().AdditionalAttackSpeed += attackSpeedIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지에 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().AdditionalAttackSpeed -= GetCurrentSumOfArray(attackSpeedIncGap);
    }
}
