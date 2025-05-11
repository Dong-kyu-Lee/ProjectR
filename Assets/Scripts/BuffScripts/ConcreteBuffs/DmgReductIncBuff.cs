using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgReductIncBuff : Buff
{
    public override BuffType BuffType => BuffType.DamageReductionIncrease;
    private float[] DmgReductIncGap = { 0.1f, 0.1f, 0.1f };  //피해감소량 증가량 간격

    public DmgReductIncBuff(float duration, GameObject target) : base(duration, target){}

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamageReduction += DmgReductIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamageReduction -= GetCurrentSumOfArray(DmgReductIncGap);
    }
}
