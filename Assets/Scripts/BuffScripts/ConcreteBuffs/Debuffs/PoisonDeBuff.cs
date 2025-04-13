using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDeBuff : Buff
{
    private float[] poisonDmg = { 1.0f, 2.0f, 5.0f }; // 레벨별 틱당 독 데미지

    public PoisonDeBuff(float duration, GameObject target) : base(duration, target) { }

    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.Hp -= poisonDmg[currentBuffLevel];
        Debug.Log($"플레이어 체력 : {playerStatus.Hp}");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        Debug.Log("독 디버프 비활성화");
    }
}
