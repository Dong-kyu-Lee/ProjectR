using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackStrategy : IAttackStrategy
{
    private EnemyAttackTiming timing;

    public MeleeAttackStrategy()
    {
        this.timing = new EnemyAttackTiming(0.85f, 0.1f, 0.6f, true, false, false);
    }

    // 생성자에서 타이밍을 외부(적 캐릭터의 상태값 등)로부터 전달받음
    public MeleeAttackStrategy(EnemyAttackTiming timing)
    {
        this.timing = timing;
    }

    public EnemyAttackTiming Timing => timing;

    public void BeginAttack(Enemy enemy)
    {
        if (enemy is BossEnemy) return;

        if (enemy.EnemyAnimator != null)
        {
            enemy.EnemyAnimator.SetTrigger("Attack");
        }
    }

    public void ExecuteAttack(Enemy enemy)
    {
        float hitOffsetX = 0.6f;

        if (enemy is BossEnemy boss && boss.MeleeHitBox != null)
        {
            boss.MeleeHitBox.transform.localPosition =
                new Vector2(-hitOffsetX, 0.3f);
            boss.MeleeHitBox.SetActive(true);
        }
        else if (enemy is MeleeEnemy meleeEnemy)
        {
            float meleeHitOffsetX = 0.35f;

            if (meleeEnemy.hitBoxObj == null) return;

            meleeEnemy.hitBoxObj.transform.localPosition = new Vector2(-meleeHitOffsetX, 0.3f);
            meleeEnemy.hitBoxObj.SetActive(true);
        }
        else if (enemy is FlyingEnemy flyingEnemy)
        {
            if (flyingEnemy.hitBoxObj == null) return;

            flyingEnemy.hitBoxObj.SetActive(true);
        }
    }
}
