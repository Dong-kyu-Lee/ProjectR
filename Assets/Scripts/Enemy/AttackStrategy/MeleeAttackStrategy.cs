using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackStrategy : IAttackStrategy
{
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
            meleeEnemy.hitBoxObj.transform.localPosition =
                new Vector2(-hitOffsetX, 0.3f);
            meleeEnemy.hitBoxObj.SetActive(true);
        }
    }
}
