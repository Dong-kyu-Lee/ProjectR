using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : IState
{
    private Enemy enemy;
    private float attackTime;
    private bool isAttacking;

    public MeleeAttackState(Enemy enemy)
    {
        this.enemy = enemy;
        attackTime = 0.7f;
        isAttacking = false;
    }

    public void Enter()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            enemy.EnemyAnimator.SetTrigger("Attack");
            enemy.StateMachine.StartCoroutine(enemy.StateMachine.AttackCoroutine(attackTime));
        }
        else
        {
            enemy.StateMachine.TransitionTo(enemy.StateMachine.chaseState);
        }
    }

    public void Update(float delta) { }

    public void FixedUpdate() { }

    public void Exit()
    {
        isAttacking = false;
    }
}
