using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Sleep;

    private float prevMoveSpeed = 0.0f;
    private float prevJumpPower = 0.0f;

    public SleepDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status status = targetObject.GetComponent<Status>();
        if (status != null)
        {
            prevMoveSpeed = status.MoveSpeed;
            status.MoveSpeed = 0.0f;
        }
        else
        {
            Debug.LogWarning("SleepDeBuff: Status 컴포넌트를 찾을 수 없습니다.");
        }

        PlayerController controller = targetObject.GetComponent<PlayerController>();
        if (controller != null)
        {
            prevJumpPower = controller.jumpPower;
            controller.jumpPower = 0.0f;
        }
        else
        {
            Debug.LogWarning("SleepDeBuff: PlayerController 컴포넌트를 찾을 수 없습니다.");
        }
    }
    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }
    public override void RemoveBuffEffect()
    {
        Status status = targetObject.GetComponent<Status>();
        if (status != null)
        {
            status.MoveSpeed = prevMoveSpeed;
        }

        PlayerController controller = targetObject.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.jumpPower = prevJumpPower;
        }
    }
}
