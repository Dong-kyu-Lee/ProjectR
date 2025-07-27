using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAttackStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy is BossEnemy boss && boss.DebuffHitBox != null)
        {
            boss.DebuffHitBox.transform.localPosition = new Vector2((boss.transform.rotation.y != 180f ? -1f : 1f), 0.3f);
            boss.DebuffHitBox.SetActive(true);
        }
        else if (enemy is DebuffMeleeEnemy debuffEnemy)
        {
            debuffEnemy.PerformAttack();
        }
    }
}
