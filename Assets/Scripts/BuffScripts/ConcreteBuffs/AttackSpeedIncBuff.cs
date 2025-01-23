using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedIncBuff : Buff
{
    private float[] attackSpeedIncGap = { 0.1f, 0.2f, 0.5f };

    public AttackSpeedIncBuff(float duration, GameObject target) : base(duration, target)
    {

    }

    public override void ApplyBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().AdditionalAttackSpeed += attackSpeedIncGap[currentBuffLevel];
        Debug.Log("공격 속도 증가" + attackSpeedIncGap[currentBuffLevel] + " 적용됨");
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().AdditionalAttackSpeed -= GetCurrentSumOfArray(attackSpeedIncGap);
        Debug.Log("공격 속도 증가" + GetCurrentSumOfArray(attackSpeedIncGap) + " 복구됨");
    }
}
