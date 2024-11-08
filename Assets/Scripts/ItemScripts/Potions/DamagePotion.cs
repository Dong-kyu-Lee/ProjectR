using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePotion : Potion
{
    [SerializeField] 
    private float dmgIncreasePercent = 10;  //피해량 증가량(퍼센트)
    public float DmgIncreasePercent { get { return dmgIncreasePercent; } }

    //피해량 증가 효과 발생
    public override void DoEffect(PlayerStatus playerStatus)
    {
        float forDebug = playerStatus.AdditionalDamage; //디버깅용
        playerStatus.AdditionalDamage +=  dmgIncreasePercent;
        Debug.Log($"피해량 {dmgIncreasePercent}% 증가함 : {forDebug} -> {playerStatus.Damage}");
        Destroy(gameObject);
    }
}
