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
    private Coroutine attackRoutine;

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

        if (enemy.EnemyAnimator != null)
        {
            enemy.EnemyAnimator.SetBool("isMove", false);
        }

        if (!isAttacking)
        {
            isAttacking = true;
            attackRoutine = enemy.StateMachine.StartCoroutine(AttackRoutine());
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
        if (attackRoutine != null)
        {
            enemy.StateMachine.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        isAttacking = false;
        enemy.isAttacking = false;
        enemy.SetAttackPhase(EnemyAttackPhase.None, false);
    }

    private IEnumerator AttackRoutine()
    {
        float applyAttackTime = Mathf.Max(attackTime, minAttackTime);
        EnemyAttackTiming timing = enemy.EnemyStatus.IsBoss ? EnemyAttackTiming.Immediate : attackStrategy.Timing;
        timing = timing.WithMinimumTotalTime(applyAttackTime);

        enemy.SetAttackPhase(EnemyAttackPhase.Windup, timing.interruptibleDuringWindup);
        if (timing.windupTime > 0f)
        {
            yield return new WaitForSeconds(timing.windupTime);
        }

        if (enemy.StateMachine.CurrentState != enemy.StateMachine.attackState)
        {
            yield break;
        }

        enemy.SetAttackPhase(EnemyAttackPhase.Active, timing.interruptibleDuringActive);
        attackStrategy.ExecuteAttack(enemy);
        if (timing.activeTime > 0f)
        {
            yield return new WaitForSeconds(timing.activeTime);
        }

        if (enemy.StateMachine.CurrentState != enemy.StateMachine.attackState)
        {
            yield break;
        }

        enemy.SetAttackPhase(EnemyAttackPhase.Recovery, timing.interruptibleDuringRecovery);
        if (timing.recoveryTime > 0f)
        {
            yield return new WaitForSeconds(timing.recoveryTime);
        }

        attackRoutine = null;
        isAttacking = false;
        enemy.isAttacking = false;
        enemy.SetAttackPhase(EnemyAttackPhase.None, false);

        if (!enemy.StateMachine.isDead && enemy.StateMachine.CurrentState == enemy.StateMachine.attackState)
        {
            enemy.StateMachine.TransitionTo(enemy.StateMachine.chaseState);
        }
    }
}
