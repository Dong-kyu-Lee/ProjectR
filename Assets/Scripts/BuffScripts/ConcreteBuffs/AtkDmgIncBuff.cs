using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class AtkDmgIncBuff : Buff
{
    private float[] atkDmgIncGap = { 0.1f, 0.2f, 0.2f };  //공격력 증가량 간격
    
    public AtkDmgIncBuff(float duration, GameObject targetObject) : base(duration, targetObject) {
        this.BuffType = BuffType.AttackDamageIncrease;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamage += atkDmgIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamage -= GetCurrentSumOfArray(atkDmgIncGap);
    }
}
