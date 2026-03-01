using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceAdditionalIncBuff : Buff
{
    private float[] priceAdditionalGap = { 10.0f, 20.0f, 50.0f };   //재화획득량 증가량 간격

    public PriceAdditionalIncBuff(float duration, GameObject target) : base(duration, target){
        this.BuffType = BuffType.PriceAdditionalIncrease;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.PriceAdditional += priceAdditionalGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 증감하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.PriceAdditional -= GetCurrentSumOfArray(priceAdditionalGap);
    }
}
