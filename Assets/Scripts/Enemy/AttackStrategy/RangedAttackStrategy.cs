using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy is RangedEnemy rangedEnemy)
        {
            rangedEnemy.ShootProjectile();
        }
    }
}
