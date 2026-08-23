using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackStrategy : IAttackStrategy
{
    private const string DefaultRangedAttackSoundPath = "Sounds/battle/queen_magic_2";

    public EnemyAttackTiming Timing
    {
        get { return new EnemyAttackTiming(0.8f, 0.1f, 0.65f, true, false, false); }
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
        switch (enemy)
        {
            case BossEnemy boss:
                boss.ShootProjectile();
                break;
            case RangedEnemy rangedEnemy:
                rangedEnemy.PlayAttackSound(DefaultRangedAttackSoundPath);
                rangedEnemy.StartCoroutine(rangedEnemy.EnableRangeAttack());
                break;
        }
    }
}
