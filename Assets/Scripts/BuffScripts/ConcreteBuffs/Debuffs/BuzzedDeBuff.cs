using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzedDeBuff : Buff
{
    private float[] moveSpeedDecGap = { 0.05f, 0.1f, 0.15f, 0.2f, 0.2f };

    public BuzzedDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            Status targetStatus = targetObject.GetComponent<Status>();
            targetStatus.AdditionalMoveSpeed -= moveSpeedDecGap[currentBuffLevel];
        }
        else
        {
            ActivateDrunkenBuff();
        }
    }

    //스택이 5스택 이상 쌓일 경우 만취 디버프로 전환
    private void ActivateDrunkenBuff()
    {
        PlayerBuffManager targetBuffManager = targetObject.GetComponent<PlayerBuffManager>();
        targetBuffManager.ActivateBuff(BuffType.Drunken, 2.0f);
        targetBuffManager.DeActivateBuff(BuffType.Buzzed);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.AdditionalMoveSpeed += GetCurrentSumOfArray(moveSpeedDecGap);
        Debug.Log("취기 디버프 해제");
    }
}
