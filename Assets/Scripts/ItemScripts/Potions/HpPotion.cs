using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPotion : Potion
{
    [SerializeField]
    private float hpIncreasePercent = 30; //체력 증가량(퍼센트)
    public float HpIncreasePercent { get { return hpIncreasePercent; } }

    //체력 회복 효과 발생
    public override void DoEffect(PlayerStatus playerStatus) 
    {
        float forDebugHP = playerStatus.Hp; //디버그용.
        playerStatus.Hp += playerStatus.Hp * hpIncreasePercent / 100.0f;
        Debug.Log($"플레이어 HP를 {hpIncreasePercent}% 즉시 회복 : {forDebugHP} -> {playerStatus.Hp}");
        Destroy(gameObject);
    }
}
