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

        if (isAttacking || attackRoutine != null) return;

        if (!isAttacking)
        {
            isAttacking = true;
            enemy.isAttacking = true;
            attackRoutine = enemy.StateMachine.StartCoroutine(AttackRoutine());
        }
    }

    public void Update(float delta)
    {
        if (attackStrategy is KnightEnemyStrategy knightStrategy)
        {
            knightStrategy.UpdateAttacking(enemy);
        }
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
        enemy.ResetAttackSoundPlayback();

        attackStrategy.BeginAttack(enemy);

        List<EnemyAttackTiming> timingsToExecute = new List<EnemyAttackTiming>();

        if (attackStrategy is IComboAttackStrategy comboStrategy && comboStrategy.IsComboAttack)
        {
            timingsToExecute.AddRange(comboStrategy.GetComboTimings());
        }
        else
        {
            EnemyAttackTiming singleTiming = enemy.EnemyStatus.IsBoss ? EnemyAttackTiming.Immediate : enemy.GetAttackTiming(attackStrategy);
            singleTiming = singleTiming.WithMinimumTotalTime(applyAttackTime);
            timingsToExecute.Add(singleTiming);
        }

        for (int i = 0; i < timingsToExecute.Count; i++)
        {
            if (attackStrategy is IComboAttackStrategy combo)
            {
                combo.SetCurrentComboIndex(i);
            }

            EnemyAttackTiming timing = timingsToExecute[i];

            // 1. Windup
            enemy.SetAttackPhase(EnemyAttackPhase.Windup, timing.interruptibleDuringWindup);
            if (timing.windupTime > 0f)
            {
                yield return new WaitForSeconds(timing.windupTime);
            }

            if (enemy.StateMachine.CurrentState != enemy.StateMachine.attackState)
            {
                yield break;
            }

            // 2. Active
            enemy.SetAttackPhase(EnemyAttackPhase.Active, timing.interruptibleDuringActive);
            if (!(enemy is MeleeEnemy))
            {
                enemy.PlayAttackSound();
            }

            attackStrategy.ExecuteAttack(enemy);
            if (timing.activeTime > 0f)
            {
                yield return new WaitForSeconds(timing.activeTime);
            }

            if (enemy.StateMachine.CurrentState != enemy.StateMachine.attackState)
            {
                yield break;
            }

            // 3. Recovery
            enemy.SetAttackPhase(EnemyAttackPhase.Recovery, timing.interruptibleDuringRecovery);
            if (timing.recoveryTime > 0f)
            {
                yield return new WaitForSeconds(timing.recoveryTime);
            }

            if (enemy.StateMachine.CurrentState != enemy.StateMachine.attackState)
            {
                yield break;
            }
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
