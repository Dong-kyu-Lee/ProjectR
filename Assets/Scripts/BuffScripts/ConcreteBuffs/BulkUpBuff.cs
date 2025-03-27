using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulkUpBuff : Buff
{
    private float[] attackDamageIncGap = { 10.0f, 20.0f, 30.0f }; //공격력 증가량 간격
    private float[] damageReduceIncGap = { 0.1f, 0.1f, 0.1f };    //데미지 감소량 간격

    public BulkUpBuff(float duration, GameObject target) : base(duration, target){}

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus status = targetObject.GetComponent<PlayerStatus>();
        status.AdditionalDamage += attackDamageIncGap[currentBuffLevel];
        status.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus status = targetObject.GetComponent<PlayerStatus>();
        status.AdditionalDamage -= GetCurrentSumOfArray(attackDamageIncGap);
        status.AdditionalDamageReduction -= GetCurrentSumOfArray(damageReduceIncGap);
    }
}
