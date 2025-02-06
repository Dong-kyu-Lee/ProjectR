using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepDeBuff : Buff
{
    private float prevMoveSpeed = 0.0f;
    private float prevJumpPower = 0.0f;
    //이쯤되면 그냥 플레이어의 canMove 같은 변수를 추가하는 게 더 효율적일 것 같음.

    public SleepDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status status = targetObject.GetComponent<Status>();
        prevMoveSpeed += status.MoveSpeed;
        status.MoveSpeed = 0.0f;

        PlayerController targetController = targetObject.GetComponent<PlayerController>();
        prevJumpPower += targetController.jumpPower;
        targetController.jumpPower = 0.0f;

        Debug.Log("수면 버프 활성화");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        Status status = targetObject.GetComponent<Status>();
        status.MoveSpeed += prevMoveSpeed;

        PlayerController targetController = targetObject.GetComponent<PlayerController>();
        targetController.jumpPower += prevJumpPower;
        Debug.Log("수면 버프 비활성화");
    }
}
