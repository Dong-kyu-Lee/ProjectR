using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Freeze;

    private float moveSpeedDec = 0.5f;      //이동속도 감소 비율
    private float speedDecAmount = 0.0f;    //감소된 이동속도 저장

    public FreezeDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null)
        {
            Debug.LogWarning("FreezeDeBuff: Status 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        if (currentBuffLevel == 0)
        {
            speedDecAmount = targetStatus.MoveSpeed * moveSpeedDec;
            targetStatus.MoveSpeed -= speedDecAmount;
            Debug.Log("빙결 디버프 : 이동속도 감소");
        }
        else if (currentBuffLevel == 4)
        {
            targetStatus.Hp -= targetStatus.Hp * 0.3f;
            currentBuffLevel--;
            Debug.Log("빙결 디버프 : 데미지 적용");
        }
        else
        {
            Debug.Log("빙결 디버프 : 스택 증가 " + currentBuffLevel);
        }
    }
    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus != null)
        {
            targetStatus.MoveSpeed += speedDecAmount;
        }
        else
        {
            Debug.LogWarning("FreezeDeBuff: Status 컴포넌트를 찾을 수 없습니다 (Remove).");
        }
    }
}
