using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCurseDeBuff : Buff
{
    private float moveSpeedDec = 10.0f;
    private float prevJumpPower = 0.0f;

    public StoneCurseDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.StoneCurse;
        maxBuffLevel = 1;
        isDebuff = true;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;

        if (targetStatus.gameObject.CompareTag("Player"))
        {
            PlayerController controller = targetObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                prevJumpPower = controller.jumpPower;
                controller.jumpPower = 0.0f;
            }
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
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed -= moveSpeedDec;

        if (targetStatus.gameObject.CompareTag("Player"))
        {
            PlayerController controller = targetObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.jumpPower = prevJumpPower;
            }
        }
    }
}
