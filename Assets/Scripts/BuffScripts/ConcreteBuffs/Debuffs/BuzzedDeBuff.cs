using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzedDeBuff : Buff
{
    private float moveSpeedDecGap = 0.02f;

    public BuzzedDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Buzzed;
        maxBuffLevel = 10;
    }

    public override void ApplyBuffEffect()
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            Status targetStatus = targetObject.GetComponent<Status>();
            targetStatus.AdditionalMoveSpeed -= moveSpeedDecGap;
        }
        else
        {
            ActivateDrunkenBuff();
        }
    }

    //스택이 5스택 이상 쌓일 경우 만취 디버프로 전환
    private void ActivateDrunkenBuff()
    {
        BuffManager targetBuffManager = targetObject.GetComponent<BuffManager>();
        targetBuffManager.ActivateBuff(BuffType.Drunken, 2.0f);
        targetBuffManager.DeactivateBuff(BuffType.Buzzed);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.AdditionalMoveSpeed += moveSpeedDecGap * (currentBuffLevel + 1);
        Debug.Log("취기 디버프 해제");
    }
}
