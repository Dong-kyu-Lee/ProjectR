using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalPercentPotion : Potion
{
    [SerializeField]
    private float criticalIncreasePercent= 0;    //크리티컬 증가량(퍼센트)
    public float CriticalIncreasePercent { get { return criticalIncreasePercent; } }

    //크리티컬 증가 효과 발생
    public override void DoEffect(PlayerStatus playerStatus)
    {
        float forDebug = playerStatus.CriticalPercent;  //디버깅용
        playerStatus.CriticalPercent += criticalIncreasePercent;
        Debug.Log($"크리티컬 증가함 : {forDebug} -> {playerStatus.CriticalPercent}");
        Destroy(gameObject);
    }
}