using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtremeSpeedBuff : Buff
{
    private float[] attackSpeedIncGap = { 10.0f, 20.0f, 30.0f };
    private float[] moveSpeedIncGap = { 10.0f, 20.0f, 30.0f };

    public ExtremeSpeedBuff(float duration, GameObject target) : base(duration, target) { }

    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        targetStatus.AdditionalAttackSpeed += attackSpeedIncGap[currentBuffLevel];
        targetStatus.MoveSpeed += moveSpeedIncGap[currentBuffLevel];
        Debug.Log("신속버프 적용됨");
    }

    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        targetStatus.AdditionalAttackSpeed -= GetCurrentSumOfArray(attackSpeedIncGap);
        targetStatus.MoveSpeed -= GetCurrentSumOfArray(moveSpeedIncGap);
        Debug.Log("신속버프 해제됨");
    }
}
