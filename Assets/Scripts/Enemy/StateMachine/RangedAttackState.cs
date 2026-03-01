using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : IState
{
    private RangedEnemy enemy;
    private float attackTime;
    private Coroutine attackCoroutine;

    public RangedAttackState(RangedEnemy enemy)
    {
        this.enemy = enemy;
        attackTime = 0.3f;
    }

    public void Enter()
    {
        if (attackCoroutine == null)
        {
            enemy.EnemyAnimator.SetTrigger("Attack");
            attackCoroutine = enemy.StateMachine.StartCoroutine(AttackCoroutine(attackTime));
        }
    }

    public void Update(float delta) { }

    public void FixedUpdate() { }

    public void Exit()
    {
        if (attackCoroutine != null)
        {
            enemy.StateMachine.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator AttackCoroutine(float delay)
    {
        Debug.Log("Coroutine");

        yield return new WaitForSeconds(0.3f);
        enemy.StartCoroutine(enemy.EnableRangeAttack());

        yield return new WaitForSeconds(delay);
        attackCoroutine = null;
        enemy.StateMachine.TransitionTo(enemy.StateMachine.chaseState);
    }
}
