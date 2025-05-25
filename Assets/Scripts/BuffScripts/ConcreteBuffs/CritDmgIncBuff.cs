using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritDmgIncBuff : Buff
{
    private float[] critDmgIncGap = { 10.0f, 20.0f, 20.0f };  //크리티컬 데미지 증가량 간격
    
    public CritDmgIncBuff(float duration , GameObject targetObj) : base(duration, targetObj) {
        this.BuffType = BuffType.CritDamageIncrease;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.CriticalDamage+= critDmgIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.CriticalDamage -= critDmgIncGap[currentBuffLevel];
    }
}
