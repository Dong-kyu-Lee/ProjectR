using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Confusion;
    public ConfusionDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Debug.Log("혼란 디버프 활성화");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        PlayerController playerController = targetObject.GetComponent<PlayerController>();
        playerController.moveFactor *= Random.Range(0, 100) < 50 ? 1.0f : -1.0f;

        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        PlayerController playerController = targetObject.GetComponent<PlayerController>();
        playerController.moveFactor *= (playerController.moveFactor > 0) ? 1.0f : -1.0f;
    }
}
