using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgReductIncBuff : Buff
{
    private float[] DmgReductIncGap = { 10.0f, 20.0f, 20.0f };  //피해감소량 증가량

    public DmgReductIncBuff(float duration, GameObject target) : base(duration, target){}

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamageReduction += DmgReductIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamageReduction -= GetCurrentSumOfArray(DmgReductIncGap);
    }
}
