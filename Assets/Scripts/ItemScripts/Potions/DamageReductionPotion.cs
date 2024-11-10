using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageReductionPotion : Potion
{
    [SerializeField]
    private float dmgReductionPercent = 10; //피해 감소량 증가량(퍼센트)
    public float DmgReductionPercent { get { return dmgReductionPercent; } }

    //피해 감소량 증가 효과 발생
    public override void DoEffect(PlayerStatus playerStatus)
    {
        float forDebug = playerStatus.AddtionalDamageReduction; //디버깅용
        playerStatus.AddtionalDamageReduction += dmgReductionPercent;
        Debug.Log($"피해 감소량 {dmgReductionPercent}% 증가함 : {forDebug} -> {playerStatus.AddtionalDamageReduction}");
                
        Destroy(gameObject);
    }
}
