using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionDeBuff : Buff
{
    public ConfusionDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Confusion;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        PlayerController playerController = targetObject.GetComponent<PlayerController>();
        if (playerController == null) return;

        playerController.moveFactor *= Random.Range(0, 100) < 50 ? 1.0f : -1.0f;

        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        PlayerController playerController = targetObject.GetComponent<PlayerController>();
        if (playerController == null) return;

        playerController.moveFactor = Mathf.Abs(playerController.moveFactor);
    }
}
