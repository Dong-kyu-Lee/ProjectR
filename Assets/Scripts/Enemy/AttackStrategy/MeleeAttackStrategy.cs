using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy is BossEnemy boss && boss.MeleeHitBox != null)
        {
            boss.MeleeHitBox.transform.localPosition = new Vector2((boss.transform.rotation.y != 180f ? -1f : 1f), 0.3f);
            boss.MeleeHitBox.SetActive(true);
        }
        else if (enemy is MeleeEnemy meleeEnemy)
        {
            meleeEnemy.PerformAttack();
        }
    }
}
