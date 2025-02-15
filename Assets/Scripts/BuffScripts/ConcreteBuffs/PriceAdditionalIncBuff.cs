using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceAdditionalIncBuff : Buff
{
    private float[] priceAdditionalGap = { 10.0f, 20.0f, 50.0f };

    public PriceAdditionalIncBuff(float duration, GameObject target) : base(duration, target){}
    public override void ApplyBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().PriceAdditional += priceAdditionalGap[currentBuffLevel];
        Debug.Log("재화증가량 증가" + priceAdditionalGap[currentBuffLevel] + " 적용됨");
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().PriceAdditional -= GetCurrentSumOfArray(priceAdditionalGap);
        Debug.Log("재화증가량 증가" + GetCurrentSumOfArray(priceAdditionalGap) + " 복구됨");
    }
}
