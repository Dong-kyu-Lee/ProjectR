using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroProxyStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy is HeroBossEnemy hero)
        {
            hero.RunCurrentStrategy();
        }
    }
}
