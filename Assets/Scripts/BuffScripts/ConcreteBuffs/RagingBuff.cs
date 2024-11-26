using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RagingBuff : Buff
{
    private float[] atkDmgIncGap = { 10.0f, 20.0f, 20.0f };     //공격력 증가량
    private float[] critDmgIncGap = { 10.0f, 20.0f, 20.0f };    //크리티컬 데미지 증가량
    private float[] critPerIncGap = { 10.0f, 20.0f, 30.0f };       //크리티컬 확률 증가량
    private float[] dmgReductDecGap = { 10.0f, 20.0f, 20.0f };     //피해 감소량 감소량

    public RagingBuff(float duration, GameObject target) : base(duration, target) { }

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamage += atkDmgIncGap[currentBuffLevel];
        playerStatus.CriticalDamage += critDmgIncGap[currentBuffLevel];
        playerStatus.CriticalPercent += critPerIncGap[currentBuffLevel];
        playerStatus.AdditionalDamageReduction -= dmgReductDecGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamage -= GetCurrentSumOfArray(atkDmgIncGap);
        playerStatus.CriticalDamage -= GetCurrentSumOfArray(critDmgIncGap);
        playerStatus.CriticalPercent -= GetCurrentSumOfArray(critPerIncGap);
        playerStatus.AdditionalDamageReduction += GetCurrentSumOfArray(dmgReductDecGap);
    }

    //현재 버프 레벨까지의 스탯 증가 누적량을 계산해주는 함수
    private float GetCurrentSumOfArray(float[] array)
    {
        float sum = 0.0f;
        for (int i = 0; i < currentBuffLevel + 1; i++)
        {
            sum += array[i];
        }
        return sum;
    }
}
