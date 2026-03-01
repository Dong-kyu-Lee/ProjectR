using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenProxyStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        // 보스라면 현재 장착된 전략(RunCurrentStrategy)을 실행하도록 위임
        if (enemy is QueenBossEnemy queen)
        {
            queen.RunCurrentStrategy();
        }
    }
}
