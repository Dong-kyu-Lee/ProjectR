using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDeBuff : Buff
{
    private float moveSpeedDec = 0.3f;

    public SlowDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Slow;
        maxBuffLevel = 1;
    }
    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;
    }
    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed += moveSpeedDec;
    }
}
