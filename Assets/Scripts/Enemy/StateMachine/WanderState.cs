using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : IState
{
    private Enemy enemy;
    private Vector2 direction;

    private float wanderTime = 1f; // 한 방향으로 이동하는 시간
    private float currentTime = 0f;


    public WanderState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.OnEdgeDetected += ChangeDirection;

        enemy.EnemyAnimator.SetBool("isMove", true);

        // 좌우 랜덤 방향 설정
        float randomValue = Random.value;
        direction = (randomValue > 0.5f) ? Vector2.right : Vector2.left;

        // 이동 시간 초기화
        currentTime = 0f;
    }

    public void Update(float delta)
    {
        currentTime += delta;

        // 일정 시간 후 Idle 상태 전환
        if (currentTime >= wanderTime && enemy.StateMachine.isChasing == false)
        {
            enemy.StateMachine.TransitionTo(enemy.StateMachine.idleState);
        }
    }

    public void FixedUpdate()
    {
        SetVelocity();
    }

    public void Exit()
    {
        enemy.OnEdgeDetected -= ChangeDirection;

        enemy.EnemyAnimator.SetBool("isMove", false);
        enemy.Rigidbody.velocity = Vector2.zero;
        direction = Vector2.zero;
    }

    void SetVelocity()
    {
        enemy.Rigidbody.velocity = new Vector2(direction.x * enemy.Speed, enemy.Rigidbody.velocity.y);
    }

    private void ChangeDirection()
    {
        direction = (direction == Vector2.right) ? Vector2.left : Vector2.right;
    }
}
