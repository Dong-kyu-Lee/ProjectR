using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RagingBuff : Buff
{
    private float[] atkDmgIncGap = { 0.1f, 0.1f, 0.1f };     //추가 피해량 증가 간격
    private float[] critDmgIncGap = { 0.2f, 0.2f, 0.2f };    //크리티컬 데미지 증가 간격
    private float[] critPerIncGap = { 0.1f, 0.1f, 0.1f };       //크리티컬 확률 증가 간격
    private float[] damageReduceIncGap = { -0.1f, -0.05f, 0f };     //피해 감소량 증가 간격

    public RagingBuff(float duration, GameObject target) : base(duration, target) {
        this.BuffType = BuffType.Raging;
        if (CalcDamage.Instance.mysteryEffect13) maxBuffLevel = 3;
        else maxBuffLevel = 2;
    }

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamage += atkDmgIncGap[currentBuffLevel];
        targetStatus.CriticalDamage += critDmgIncGap[currentBuffLevel];
        targetStatus.CriticalPercent += critPerIncGap[currentBuffLevel];
        if (currentBuffLevel > 0)
        {
            targetStatus.AdditionalDamageReduction -= damageReduceIncGap[currentBuffLevel - 1]; ;
            targetStatus.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];
        }
        else targetStatus.AdditionalDamageReduction += damageReduceIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalDamage -= GetCurrentSumOfArray(atkDmgIncGap);
        targetStatus.CriticalDamage -= GetCurrentSumOfArray(critDmgIncGap);
        targetStatus.CriticalPercent -= GetCurrentSumOfArray(critPerIncGap);
        targetStatus.AdditionalDamageReduction -= damageReduceIncGap[currentBuffLevel];
    }
}
