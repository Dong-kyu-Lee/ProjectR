using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCurseDeBuff : Buff
{
    private float prevMoveSpeed = 0.0f;
    private float prevJumpPower = 0.0f;

    public StoneCurseDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.StoneCurse;
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus != null)
        {
            prevMoveSpeed = targetStatus.MoveSpeed;
            targetStatus.MoveSpeed = 0.0f;
        }
        else
        {
            Debug.LogWarning("StoneCurseDeBuff: Status 컴포넌트가 없습니다.");
        }
        PlayerController targetController = targetObject.GetComponent<PlayerController>();
        if (targetController != null)
        {
            prevJumpPower = targetController.jumpPower;
            targetController.jumpPower = 0.0f;
        }
        else
        {
            Debug.LogWarning("StoneCurseDeBuff: PlayerController 컴포넌트가 없습니다.");
        }
    }
    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus != null)
        {
            targetStatus.MoveSpeed = prevMoveSpeed;
        }

        PlayerController targetController = targetObject.GetComponent<PlayerController>();
        if (targetController != null)
        {
            targetController.jumpPower = prevJumpPower;
        }
    }
}
