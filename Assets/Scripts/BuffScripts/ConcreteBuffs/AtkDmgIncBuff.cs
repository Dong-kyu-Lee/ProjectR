using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class AtkDmgIncBuff : Buff
{
    private float[] atkDmgIncVal = { 10.0f, 30.0f, 50.0f };
    
    public AtkDmgIncBuff(float duration, GameObject targetObject) : 
        base(duration, targetObject) {}

    //버프를 target에게 적용시키는 메서드.
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamage += atkDmgIncVal[currentBuffLevel];
    }

    //target에게 적용된 버프를 제거하는 메서드
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamage -= atkDmgIncVal[currentBuffLevel];
        Debug.Log("공격력 증가 버프 해제");
    }

    //버프를 중첩시키는 메서드
    public override void BuffOverlap(float duration)
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            currentBuffLevel++;
            BuffUpgrade();
        }
        currentDuration += duration;
    }

    //버프를 업그레이드 후 타겟에게 적용시키는 메서드
    protected override void BuffUpgrade()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamage += (atkDmgIncVal[currentBuffLevel] - atkDmgIncVal[currentBuffLevel - 1]);
        Debug.Log($"버프 적용 및 업그레이드 됨 : 공격력 버프 LV.{currentBuffLevel + 1}");
    }
}
