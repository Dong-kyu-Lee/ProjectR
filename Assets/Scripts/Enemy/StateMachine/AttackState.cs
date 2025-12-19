using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private Enemy enemy;
    private IAttackStrategy attackStrategy;
    private float attackTime;
    private float minAttackTime;
    private bool isAttacking;

    public AttackState(Enemy enemy, IAttackStrategy attackStrategy)
    {
        this.enemy = enemy;
        attackTime = 1 / enemy.EnemyStatus.TotalAttackSpeed;
        minAttackTime = 0.1f;
        isAttacking = false;
        this.attackStrategy = attackStrategy;
    }

    public void Enter()
    {
        enemy.isAttacking = true;
        if (!isAttacking)
        {
            isAttacking = true;
            attackStrategy.ExecuteAttack(enemy);
            float applyAttackTime = attackTime;
            if (attackTime < minAttackTime) applyAttackTime = minAttackTime;
            enemy.StateMachine.StartCoroutine(enemy.StateMachine.AttackCoroutine(applyAttackTime));
        }
    }

    public void Update(float delta)
    {

    }

    public void FixedUpdate()
    {

    }

    public void Exit()
    {
        isAttacking = false;
    }
}
