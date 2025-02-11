using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBodyBuff : Buff
{
    private float[] damageReduceGap = { 10.0f, 20.0f, 20.0f };  //피해감소량 증가량
    private float[] attackSpeedDecGap = { 10.0f, 20.0f, 30.0f };  //공격속도 감소량 증가량
    private float[] moveSpeedDecGap = { 2.0f, 3.0f, 5.0f };  //이동속도 감소량 증가량

    public IronBodyBuff(float totalDuration, GameObject target) : base(totalDuration, target) { }
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.AdditionalDamageReduction += damageReduceGap[currentBuffLevel];
        playerStatus.AdditionalAttackSpeed -= attackSpeedDecGap[currentBuffLevel];
        playerStatus.MoveSpeed -= moveSpeedDecGap[currentBuffLevel];
        Debug.Log("강철몸 버프 적용");
    }

    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.AdditionalDamageReduction -= GetCurrentSumOfArray(damageReduceGap);
        playerStatus.AdditionalAttackSpeed += GetCurrentSumOfArray(attackSpeedDecGap);
        playerStatus.MoveSpeed += GetCurrentSumOfArray(moveSpeedDecGap);
        Debug.Log("강철몸 버프 해제");
    }
}
