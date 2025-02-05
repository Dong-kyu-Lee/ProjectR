using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDeBuff : Buff
{
    private float prevMoveSpeed = 0.0f;
    //private float prevJumpPower = 0.0f;

    public StunDeBuff(float duration, GameObject target) : base(duration, target)
    { 
        maxBuffLevel = 1; 
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        prevMoveSpeed += targetStatus.MoveSpeed;
        targetStatus.MoveSpeed = 0.0f;
        //prevJumpPower += targetStatus.JumpPower;
        //targetStatus.JumpPower = 0.0f;
        Debug.Log("기절 디버프 활성화");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.MoveSpeed += prevMoveSpeed;
        //targetStatus.JumpPower += prevJumpPower;
        Debug.Log("기절 디버프 비활성화");
    }
}
