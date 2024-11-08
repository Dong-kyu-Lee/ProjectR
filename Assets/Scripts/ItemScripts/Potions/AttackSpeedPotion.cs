using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedPotion : Potion
{
    [SerializeField] 
    private float attackSpeedIncreasement = 10;    //공격속도 증가량 (퍼센트)
    public float AttackSpeedIncreasement { get { return attackSpeedIncreasement; } }

    //공격속도 증가 효과 발생
    public override void DoEffect(PlayerStatus playerStatus)
    {
        float forDebug = playerStatus.TotalAttackSpeed; //디버깅용
        playerStatus.AdditionalAttackSpeed += attackSpeedIncreasement;
        Debug.Log($"공격속도 {attackSpeedIncreasement}% 증가함: {forDebug} -> {playerStatus.TotalAttackSpeed}");
    }
}
