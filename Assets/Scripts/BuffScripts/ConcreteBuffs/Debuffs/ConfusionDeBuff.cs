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
        Debug.Log("혼란 디버프 활성화");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        PlayerController playerController = GameManager.Instance.CurrentPlayer.GetComponent<PlayerController>();
        playerController.moveFactor *= Random.Range(0, 100) < 50 ? 1.0f : -1.0f;

        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        PlayerController playerController = GameManager.Instance.CurrentPlayer.GetComponent<PlayerController>();
        playerController.moveFactor *= (playerController.moveFactor > 0) ? 1.0f : -1.0f;
    }
}
