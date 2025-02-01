using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleBuff : Buff
{
    private float prevDamageReduction = 0.0f;
    public InvincibleBuff(float totalDuration, GameObject target) : base(totalDuration, target) { }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        prevDamageReduction = targetStatus.DamageReduction;
        targetStatus.DamageReduction = 1;
        Debug.Log("무적버프 활성화");
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<Status>().DamageReduction = prevDamageReduction;
        Debug.Log("무적버프 비활성화");
    }
}
