using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDeBuff : Buff
{
    private float prevMoveSpeed = 0.0f;
    private float prevJumpPower = 0.0f;

    public StunDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Stun;
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
            Debug.LogWarning("StunDeBuff: Status 컴포넌트를 찾을 수 없습니다.");
        }
        PlayerController targetController = targetObject.GetComponent<PlayerController>();
        if (targetController != null)
        {
            prevJumpPower = targetController.jumpPower;
            targetController.jumpPower = 0.0f;
        }
        else
        {
            Debug.LogWarning("StunDeBuff: PlayerController 컴포넌트를 찾을 수 없습니다.");
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
