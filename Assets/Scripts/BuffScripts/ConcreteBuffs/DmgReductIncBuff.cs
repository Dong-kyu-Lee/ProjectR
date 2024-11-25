using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgReductIncBuff : Buff
{
    private float[] DmgReductIncGap = { 10.0f, 20.0f, 20.0f };  //피해감소량 증가량

    public DmgReductIncBuff(float duration, GameObject target) : base(duration, target){}

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AddtionalDamageReduction += DmgReductIncGap[currentBuffLevel];
        Debug.Log("방어력 증가 버프 적용됨");
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.AddtionalDamageReduction -= GetCurrentSumOfArray(DmgReductIncGap);
        Debug.Log("방어력 증가 버프 해제");
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
