using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    private Enemy enemy;
    private float stunDuration;
    private float currentTimer;

    public StunState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    // 외부에서 경직 시간 세팅
    public void SetDuration(float duration)
    {
        stunDuration = duration;
        currentTimer = 0f;
    }

    public void Enter()
    {
        // 적의 물리 이동 정지
        enemy.Rigidbody.velocity = Vector2.zero;

        // 혹시 이동 애니메이션이 켜져있다면 끔
        if (enemy.EnemyAnimator != null)
        {
            enemy.EnemyAnimator.SetBool("isMove", false);
            // enemy.EnemyAnimator.SetTrigger("Hit"); // 피격 애니가 있다면 주석 해제
        }
    }

    public void Update(float delta)
    {
        // 경직 시간 체크
        currentTimer += delta;
        if (currentTimer >= stunDuration)
        {
            // 경직이 끝나면 상태 복구
            if (enemy.PlayerTransform != null)
            {
                enemy.StateMachine.TransitionTo(enemy.StateMachine.chaseState);
            }
            else
            {
                enemy.StateMachine.TransitionTo(enemy.StateMachine.idleState);
            }
        }
    }

    public void FixedUpdate() { }
    public void Exit() { }
}
