using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Sleep;
    //그냥 타겟에 canMove 같은 변수를 추가하여 움직임을 막는 게 더 효율적일 것 같음.

    private float prevMoveSpeed = 0.0f;     //Sleep 전 플레이어가 가지고 있던 이동속도 양
    private float prevJumpPower = 0.0f;     //Sleep 전 플레이어가 가지고 있던 점프력 양
    

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
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        Status status = targetObject.GetComponent<Status>();
        status.MoveSpeed += prevMoveSpeed;      //이동속도 되돌리기

        PlayerController targetController = targetObject.GetComponent<PlayerController>();
        targetController.jumpPower += prevJumpPower;    //점프력 되돌리기
    }
}
