using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private Enemy enemy;
    private IAttackStrategy attackStrategy;
    private float attackTime;
    private bool isAttacking;

    public AttackState(Enemy enemy, IAttackStrategy attackStrategy)
    {
        this.enemy = enemy;
        attackTime = 0.7f;
        isAttacking = false;
        this.attackStrategy = attackStrategy;
    }

    public void Enter()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            enemy.EnemyAnimator.SetTrigger("Attack");
            attackStrategy.ExecuteAttack(enemy);
            enemy.StateMachine.StartCoroutine(enemy.StateMachine.AttackCoroutine(attackTime));
        }
        else
        {
            enemy.StateMachine.TransitionTo(enemy.StateMachine.chaseState);
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
