using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeDeBuff : Buff
{
    private float moveSpeedDec = 0.5f;
    private float speedDecAmount = 0.0f;

    public FreezeDeBuff(float duration, GameObject target) : base(duration, target) 
    {
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        if(currentBuffLevel == 0)
        {
            Status targetStatus = targetObject.GetComponent<Status>();
            speedDecAmount = targetStatus.MoveSpeed * moveSpeedDec;
            targetStatus.MoveSpeed -= speedDecAmount;
            Debug.Log("빙결 디버프 : 이동속도 감소");
        }
        else if(currentBuffLevel == 4)
        {   //5번째 스택일 때 데미지 적용
            Status targetStatus = targetObject.GetComponent<Status>();
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
        targetObject.GetComponent<Status>().MoveSpeed += speedDecAmount;
    }
}
