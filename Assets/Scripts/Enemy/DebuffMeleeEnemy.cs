using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffMeleeEnemy : MeleeEnemy
{
    public BuffType debuffType;
    public float debuffDuration = 3f;

    protected override void Awake()
    {
        base.Awake();
        SetAttackStrategy(new DebuffAttackStrategy());
    }

    // HitBox 충돌 이벤트에서 호출될 수 있음
    public override void ApplyDebuff(GameObject target)
    {
        BuffManager playerBuffManager = target.GetComponent<BuffManager>();
        if (playerBuffManager != null)
        {
            playerBuffManager.ActivateDeBuff(debuffType, debuffDuration);
        }
    }
}
