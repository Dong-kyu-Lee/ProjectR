using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleEyeBuff : Buff
{
    private float[] critPercentIncGap = { 0.2f, 0.3f, 0.5f };
    private float[] critDamageIncGap = { 0.5f, 1.0f, 1.5f };

    public EagleEyeBuff(float duration, GameObject target) : base(duration, target) { }

    public override void ApplyBuffEffect()
    {
        PlayerStatus status = targetObject.GetComponent<PlayerStatus>();
        status.CriticalPercent += critPercentIncGap[currentBuffLevel];
        status.CriticalDamage += critDamageIncGap[currentBuffLevel];
        Debug.Log("매의눈 버프 적용됨");
    }

    public override void RemoveBuffEffect()
    {
        PlayerStatus status = targetObject.GetComponent<PlayerStatus>();
        status.CriticalPercent -= GetCurrentSumOfArray(critPercentIncGap);
        status.CriticalDamage -= GetCurrentSumOfArray(critDamageIncGap);
        Debug.Log("매의눈 버프 해제됨");
    }
}
