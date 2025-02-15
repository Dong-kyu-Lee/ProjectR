using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritDmgIncBuff : Buff
{
    private float[] critDmgIncGap = { 10.0f, 20.0f, 20.0f };  //크리티컬 데미지 증가량
    
    public CritDmgIncBuff(float duration , GameObject targetObj) : base(duration, targetObj) { }
    public override void ApplyBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().CriticalDamage += critDmgIncGap[currentBuffLevel];
        Debug.Log("크리티컬 데미지 증가" + critDmgIncGap[currentBuffLevel] + " 적용됨");
    }

    public override void RemoveBuffEffect()
    {
        targetObject.GetComponent<PlayerStatus>().CriticalDamage -= GetCurrentSumOfArray(critDmgIncGap);
        Debug.Log("크리티컬 데미지 증가" + GetCurrentSumOfArray(critDmgIncGap) + " 복구됨");
    }
}
