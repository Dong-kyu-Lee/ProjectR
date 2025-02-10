using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritPercentIncBuff : Buff
{
    private float[] critPercentIncGap = { 0.1f, 0.2f, 0.3f };  //크리티컬 확률 증가량

    public CritPercentIncBuff(float duration, GameObject targetObj) : base(duration, targetObj) { }


    public override void ApplyBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().CriticalPercent += critPercentIncGap[currentBuffLevel];
        Debug.Log("크리티컬 확률 증가" + critPercentIncGap[currentBuffLevel] + " 적용됨");
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().CriticalPercent -= GetCurrentSumOfArray(critPercentIncGap);
        Debug.Log("크리티컬 확률 증가" + GetCurrentSumOfArray(critPercentIncGap) + " 복구됨");
    }
}
