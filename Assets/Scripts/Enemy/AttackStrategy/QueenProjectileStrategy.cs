using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenProjectileStrategy : IAttackStrategy
{
    public void ExecuteAttack(Enemy enemy)
    {
        // 들어온 enemy가 QueenBossEnemy인지 확인하고 전용 함수 호출
        if (enemy is QueenBossEnemy queen)
        {
            queen.CastMagicAttack();
        }
        else
        {
            Debug.LogWarning("QueenProjectileStrategy는 QueenBossEnemy에게만 할당해야 합니다.");
        }
    }
}
