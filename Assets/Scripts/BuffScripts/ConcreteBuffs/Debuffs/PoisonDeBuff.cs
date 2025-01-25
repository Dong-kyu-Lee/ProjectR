using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDeBuff : Buff
{
    private float[] PosionDamage = { 1.0f, 2.0f, 5.0f };

    public PoisonDeBuff(float duration, GameObject target) : base(duration, target) { }

    public override void ApplyBuffEffect()
    {

    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();
        playerStatus.Hp -= PosionDamage[currentBuffLevel];
        Debug.Log($"플레이어 체력 : {playerStatus.Hp}");
        CurrentDuration -= tickDuration;
    }

    public override void RemoveBuffEffect()
    {

    }
}
