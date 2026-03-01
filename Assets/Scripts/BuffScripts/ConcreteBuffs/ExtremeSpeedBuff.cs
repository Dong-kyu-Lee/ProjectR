using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtremeSpeedBuff : Buff
{
    private float[] attackSpeedIncGap = { 0.2f, 0.4f, 0.4f };    //공격속도 증가량 간격
    private float[] moveSpeedIncGap = { 0.05f, 0.1f, 0.1f };      //이동속도 증가량 간격

    public ExtremeSpeedBuff(float duration, GameObject target) : base(duration, target) {
        this.BuffType = BuffType.ExtremeSpeed;
        if (CalcDamage.Instance.mysteryEffect13) maxBuffLevel = 3;
        else maxBuffLevel = 2;
    }

    //대상에게 버프를 적용하는 함수. 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalAttackSpeed += attackSpeedIncGap[currentBuffLevel];
        targetStatus.AdditionalMoveSpeed += moveSpeedIncGap[currentBuffLevel];
    }

    //적용된 버프를 해제하는 함수. currentBuffLevel까지 해당하는 간격 값을 합산한 후 감소하는 식
    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.AdditionalAttackSpeed -= GetCurrentSumOfArray(attackSpeedIncGap);
        targetStatus.AdditionalMoveSpeed -= GetCurrentSumOfArray(moveSpeedIncGap);
    }
}
