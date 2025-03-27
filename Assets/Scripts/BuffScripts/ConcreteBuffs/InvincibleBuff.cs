using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleBuff : Buff
{
    private float prevDamageReduction = 0.0f;   //이전 데미지 리덕션 값
    public InvincibleBuff(float totalDuration, GameObject target) : base(totalDuration, target) { }

    public override void ApplyBuffEffect()
    {
        //데미지 리덕션 값이 1.0f가 되면 무적이 되는지 의문임. 추후 수정 필요할지도
        Status targetStatus = targetObject.GetComponent<Status>();
        prevDamageReduction = targetStatus.DamageReduction;
        targetStatus.DamageReduction = 1.0f;
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<Status>().DamageReduction = prevDamageReduction;
    }
}
