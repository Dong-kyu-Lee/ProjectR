using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtremeSpeedBuff : Buff
{
    private float[] attackSpeedIncGap = { 10.0f, 20.0f, 30.0f };    //공격속도 증가량 간격
    private float[] moveSpeedIncGap = { 10.0f, 20.0f, 30.0f };      //이동속도 증가량 간격

    public ExtremeSpeedBuff(float duration, GameObject target) : base(duration, target) {
        this.BuffType = BuffType.ExtremeSpeed;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.AdditionalAttackSpeed += attackSpeedIncGap[currentBuffLevel];
        playerStatus.MoveSpeed += moveSpeedIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.AdditionalAttackSpeed -= GetCurrentSumOfArray(attackSpeedIncGap);
        playerStatus.MoveSpeed -= GetCurrentSumOfArray(moveSpeedIncGap);
    }
}
