using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgReductIncBuff : Buff
{
    private float[] damageReduceIncGap = { 0.05f, 0.1f, 0.15f };  //피해 감소량 증가 간격

    public DmgReductIncBuff(float duration, GameObject target) : base(duration, target){
        this.BuffType = BuffType.DamageReductionIncrease;
        if (CalcDamage.Instance.mysteryEffect13) maxBuffLevel = 3;
        else maxBuffLevel = 2;
    }

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        if (currentBuffLevel > 0)
        {
            targetStatus.AdditionalDamageReduction -= damageReduceIncGap[currentBuffLevel - 1];
            targetStatus.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];
        }
        else targetStatus.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamageReduction -= damageReduceIncGap[currentBuffLevel];
    }
}
