using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCurseDeBuff : Buff
{
    //Sleep 디버프와 마찬가지로 canMove 같은 변수를 추가하여 움직임을 막는 게 더 효율적일 것 같음.

    private float prevMoveSpeed = 0.0f;     //석화 전 플레이어가 가지고 있던 이동속도 양
    private float prevJumpPower = 0.0f;     //석화 전 플레이어가 가지고 있던 점프력 양

    public StoneCurseDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        prevMoveSpeed += playerStatus.MoveSpeed;
        playerStatus.MoveSpeed = 0.0f;

        PlayerController targetController = GameManager.Instance.CurrentPlayer.GetComponent<PlayerController>();
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
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.MoveSpeed += prevMoveSpeed;    //감소시킨 양만큼 이동속도 되돌리기

        PlayerController targetController = GameManager.Instance.CurrentPlayer.GetComponent<PlayerController>();
        targetController.jumpPower += prevJumpPower;    //감소시킨 양만큼 점프력 되돌리기
    }
}
