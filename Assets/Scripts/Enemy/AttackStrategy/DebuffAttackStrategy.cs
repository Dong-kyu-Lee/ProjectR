using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAttackStrategy : IAttackStrategy
{
    public EnemyAttackTiming Timing
    {
        get { return new EnemyAttackTiming(0.3f, 0.1f, 0.7f, true, false, false); }
    }

    public void ExecuteAttack(Enemy enemy)
    {
        enemy.EnemyAnimator.SetTrigger("Attack");

        if (enemy is BossEnemy boss && boss.DebuffHitBox != null)
        {
            float hitOffsetX = 0.6f;
            boss.DebuffHitBox.transform.localPosition =
                new Vector2(-hitOffsetX, 0.3f);
            boss.DebuffHitBox.SetActive(true);
        }
        else if (enemy is DebuffMeleeEnemy debuffEnemy)
        {
            debuffEnemy.PerformAttack();
        }
    }
}
