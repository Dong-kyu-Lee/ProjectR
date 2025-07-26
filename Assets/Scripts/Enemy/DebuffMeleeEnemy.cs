using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffMeleeEnemy : MeleeEnemy
{
    public float debuffDuration = 3f;
    public float debuffAmount = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        SetAttackStrategy(new DebuffAttackStrategy());
    }

    // HitBox 충돌 이벤트에서 호출될 수 있음
    public void ApplyDebuff(GameObject target)
    {
        PlayerStatus status = target.GetComponent<PlayerStatus>();
        if (status != null)
        {
            //status.TakeDamage(this.gameObject, enemyStatus.EnemyStatusData.Atk, 0, false);
            //status.ApplyDebuff(debuffAmount, debuffDuration);
        }
    }
}
