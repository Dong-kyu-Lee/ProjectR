using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDeBuff : Buff
{
    //StoneCurse, Sleep 디버프와 마찬가지로 canMove 같은 변수를 추가하여 움직임을 막는 게 더 효율적일 것 같음.

    private float prevMoveSpeed = 0.0f;     //Stun 전 플레이어가 가지고 있던 이동속도 양
    private float prevJumpPower = 0.0f;     //Stun 전 플레이어가 가지고 있던 점프력 양

    public StunDeBuff(float duration, GameObject target) : base(duration, target)
    { 
        maxBuffLevel = 1; 
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        prevMoveSpeed += targetStatus.MoveSpeed;
        targetStatus.MoveSpeed = 0.0f;
        
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
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.MoveSpeed += prevMoveSpeed;

        PlayerController targetController = targetObject.GetComponent<PlayerController>();
        targetController.jumpPower += prevJumpPower;
    }
}
