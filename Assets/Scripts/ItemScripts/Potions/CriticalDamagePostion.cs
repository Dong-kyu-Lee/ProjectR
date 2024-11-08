using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalDamagePostion : Potion
{
    [SerializeField]
    private float criticalDamageIncrease = 10;  //크리티컬 데미지 증가량(퍼센트)
    public float CriticalDamageIncrease { get { return criticalDamageIncrease; } }

    //크리티컬 데미지 증가 효과 발생
    public override void DoEffect(PlayerStatus playerStatus)
    {
        float forDebug = playerStatus.CriticalDamage;
        playerStatus.CriticalDamage += playerStatus.CriticalDamage * criticalDamageIncrease * 0.01f;
        Debug.Log($"크리티컬 데미지 {criticalDamageIncrease}% 증가함 : {forDebug} -> {playerStatus.CriticalDamage}");
    }
}
