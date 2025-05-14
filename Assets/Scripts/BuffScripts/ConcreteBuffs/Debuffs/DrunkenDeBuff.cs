using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Drunken;
    private float[] moveSpeedDecGap = { 0.1f, 0.2f, 0.2f };         //이동속도 감소량의 간격
    private float[] attackSpeedDecGap = { 10.0f, 10.0f, 30.0f };    //공격속도 감소량의 간격

    public DrunkenDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 3;
    }

    public override void ApplyBuffEffect()
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            Status targetStatus = targetObject.GetComponent<Status>();
            if (targetStatus != null)
            {
                targetStatus.MoveSpeed -= moveSpeedDecGap[currentBuffLevel];
                targetStatus.AttackSpeed -= attackSpeedDecGap[currentBuffLevel];
            }
            else
            {
                BuffManager targetBuffManager = targetObject.GetComponent<BuffManager>();
                if (targetBuffManager != null)
                {
                    targetBuffManager.ActivateBuff(BuffType.Sleep, 10.0f);
                }
                else
                {
                    Debug.LogWarning("DrunkenDeBuff: BuffManager 컴포넌트가 없습니다.");
                }
            }
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
        BuffManager targetBuffManager = targetObject.GetComponent<BuffManager>();
        targetBuffManager.ActivateBuff(BuffType.Sleep, 10.0f);

    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus != null)
        {
            targetStatus.MoveSpeed += GetCurrentSumOfArray(moveSpeedDecGap);
            targetStatus.AttackSpeed += GetCurrentSumOfArray(attackSpeedDecGap);
        }
        else
        {
            Debug.LogWarning("DrunkenDeBuff: Status 컴포넌트가 없습니다 (Remove).");
        }

        Debug.Log("만취 디버프 해제");
    }
}
