using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenDeBuff : Buff
{
    private float prevMoveSpeed = 0.0f;

    public DrunkenDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        prevMoveSpeed += targetStatus.MoveSpeed;
        targetStatus.MoveSpeed = 0.0f;
        Debug.Log("만취 디버프 부여");
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
        Debug.Log("만취 디버프 해제");
    }
}
