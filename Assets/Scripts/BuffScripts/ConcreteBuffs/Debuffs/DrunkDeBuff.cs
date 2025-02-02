using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenDeBuff : Buff
{
    private float[] moveSpeedDecGap = { 0.1f, 0.2f, 0.2f };
    private float[] attackSpeedDecGap = { 10.0f, 10.0f, 30.0f };

    public DrunkenDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 3;
    }

    public override void ApplyBuffEffect()
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            Status targetStatus = targetObject.GetComponent<Status>();
            targetStatus.MoveSpeed -= moveSpeedDecGap[currentBuffLevel];
            targetStatus.AttackSpeed -= attackSpeedDecGap[currentBuffLevel];
        }
        else
        {
            ActivateSleepBuff();
        }
        Debug.Log("만취 디버프 부여");

    }

    //스택이 3스택 이상 쌓일 경우 슬립 디버프로 전환시키는 메서드
    private void ActivateSleepBuff()
    {
        PlayerBuffManager targetBuffManager = targetObject.GetComponent<PlayerBuffManager>();
        targetBuffManager.ActivateBuff(BuffType.Sleep, 10.0f);

    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        targetStatus.MoveSpeed += GetCurrentSumOfArray(moveSpeedDecGap);
        targetStatus.AttackSpeed += GetCurrentSumOfArray(attackSpeedDecGap);
        Debug.Log("만취 디버프 해제");
    }
}
