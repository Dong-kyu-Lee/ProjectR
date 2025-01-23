using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulkUpBuff : Buff
{
    private float[] attackDamageIncGap = { 10.0f, 20.0f, 30.0f };
    private float[] damageReduceIncGap = { 10.0f, 20.0f, 30.0f };

    public BulkUpBuff(float duration, GameObject target) : base(duration, target){}

    public override void ApplyBuffEffect()
    {
        PlayerStatus status = targetObject.GetComponent<PlayerStatus>();
        status.AdditionalDamage += attackDamageIncGap[currentBuffLevel];
        status.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];
    }

    public override void RemoveBuffEffect()
    {
        PlayerStatus status = targetObject.GetComponent<PlayerStatus>();
        status.AdditionalDamage -= GetCurrentSumOfArray(attackDamageIncGap);
        status.AdditionalDamageReduction -= GetCurrentSumOfArray(damageReduceIncGap);
    }
}
