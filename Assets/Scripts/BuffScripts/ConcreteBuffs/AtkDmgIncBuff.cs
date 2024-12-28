using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class AtkDmgIncBuff : Buff
{
    private float[] atkDmgIncGap = { 10.0f, 20.0f, 20.0f };  //공격력 증가량
    
    public AtkDmgIncBuff(float duration, GameObject targetObject) : 
        base(duration, targetObject) {}

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        Debug.Log($"공격력 증가 전 : {playerStatus.AdditionalDamage}");
        playerStatus.AdditionalDamage += atkDmgIncGap[currentBuffLevel];
        Debug.Log($"공격력 증가 후 : {playerStatus.AdditionalDamage}");
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AdditionalDamage -= GetCurrentSumOfArray(atkDmgIncGap);
        Debug.Log($"공격력 증가 복구 : {playerStatus.AdditionalDamage}");
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
