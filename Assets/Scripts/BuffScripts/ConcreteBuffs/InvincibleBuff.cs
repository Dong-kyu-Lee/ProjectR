using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleBuff : Buff
{
    private float prevDamageReduction = 0.0f;
    public InvincibleBuff(float totalDuration, GameObject target) : base(totalDuration, target) { }

    public override void ApplyBuffEffect()
    {
        //데미지 리덕션 값이 1.0f가 되면 무적이 되는지 의문임. 추후 수정 필요
        Status targetStatus = targetObject.GetComponent<Status>();
        prevDamageReduction = targetStatus.DamageReduction;
        targetStatus.DamageReduction = 1.0f;
        Debug.Log("무적버프 활성화 => 데미지 리덕션 값 1.0f로 만듦.");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<Status>().DamageReduction = prevDamageReduction;
        Debug.Log("무적버프 비활성화");
    }
}
