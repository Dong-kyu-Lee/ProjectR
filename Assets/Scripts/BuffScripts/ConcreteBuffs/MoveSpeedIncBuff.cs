using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedIncBuff : Buff
{
    private float[] moveSpeedIncGap = { 10.0f, 40.0f, 50.0f };
    public MoveSpeedIncBuff(float duration, GameObject target) : base(duration, target) { }

    public override void ApplyBuffEffect()
    {
        targetObject.GetComponent<Status>().MoveSpeed += moveSpeedIncGap[currentBuffLevel];
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().MoveSpeed -= GetCurrentSumOfArray(moveSpeedIncGap);
        Debug.Log("이동 속도 증가" + GetCurrentSumOfArray(moveSpeedIncGap) + " 복구됨");
    }
}
