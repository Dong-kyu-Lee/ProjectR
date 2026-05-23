using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleBuff : Buff
{
    public InvincibleBuff(float totalDuration, GameObject target) : base(totalDuration, target) {
        this.BuffType = BuffType.Invincible;
        maxBuffLevel = 1;
        maxDuration = 5;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.Invincible = true;
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.Invincible = false;
    }
}
