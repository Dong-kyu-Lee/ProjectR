using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleEyeBuff : Buff
{
    public override BuffType BuffType => BuffType.EagleEye;
    private float[] critPercentIncGap = { 0.2f, 0.3f, 0.5f };   //크리티컬 확률 증가량 간격
    private float[] critDamageIncGap = { 0.5f, 1.0f, 1.5f };   //크리티컬 데미지 증가량 간격

    public EagleEyeBuff(float duration, GameObject target) : base(duration, target) {
        this.BuffType = BuffType.EagleEye;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return; 
        playerStatus.CriticalPercent += critPercentIncGap[currentBuffLevel];
        playerStatus.CriticalDamage += critDamageIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.CriticalPercent -= GetCurrentSumOfArray(critPercentIncGap);
        playerStatus.CriticalDamage -= GetCurrentSumOfArray(critDamageIncGap);
    }
}
