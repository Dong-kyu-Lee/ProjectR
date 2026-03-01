using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy.EnemyAnimator != null)
        {
            enemy.EnemyAnimator.SetTrigger("Attack");
        }

        switch (enemy)
        {
            case BossEnemy boss:
                boss.ShootProjectile();
                break;
            case RangedEnemy rangedEnemy:
                rangedEnemy.StartCoroutine(rangedEnemy.EnableRangeAttack());
                break;
        }
    }
}
