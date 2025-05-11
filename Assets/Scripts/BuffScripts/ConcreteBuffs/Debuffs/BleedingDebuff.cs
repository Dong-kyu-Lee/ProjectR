using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingDebuff : Buff
{
    private float[] bleedingDmg = { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f};    //레벨별 틱당 출혈 데미지

    public BleedingDebuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Bleeding;
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        Debug.Log("출혈 디버프 활성화"); 
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.Hp -= bleedingDmg[currentBuffLevel];
        InGameUIManager.Instance.CheckHp();
        Debug.Log($"출혈 후 플레이어 체력 : {playerStatus.Hp}");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        Debug.Log("출혈 디버프 비활성화");
    }
}