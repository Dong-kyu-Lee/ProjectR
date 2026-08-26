using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAttackStrategy : IAttackStrategy
{
    public EnemyAttackTiming Timing
    {
        get { return new EnemyAttackTiming(0.85f, 0.1f, 0.7f, true, false, false); }
    }

    public void BeginAttack(Enemy enemy)
    {
        if (enemy.EnemyAnimator != null)
        {
            enemy.EnemyAnimator.SetTrigger("Attack");
        }
    }

    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy is BossEnemy boss && boss.DebuffHitBox != null)
        {
            float hitOffsetX = 0.6f;
            boss.DebuffHitBox.transform.localPosition =
                new Vector2(-hitOffsetX, 0.3f);
            boss.DebuffHitBox.SetActive(true);
        }
        else if (enemy is DebuffMeleeEnemy debuffEnemy)
        {
            float hitOffsetX = 0.35f;

            if (debuffEnemy.hitBoxObj == null) return;

            debuffEnemy.hitBoxObj.transform.localPosition = new Vector2(-hitOffsetX, 0.3f);
            debuffEnemy.hitBoxObj.SetActive(true);
        }
    }
}
