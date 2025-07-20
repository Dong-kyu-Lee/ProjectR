using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAttackStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy is MeleeEnemy meleeEnemy)
        {
            meleeEnemy.PerformAttack();
        }
    }
}
