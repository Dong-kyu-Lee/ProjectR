using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private Enemy enemy;
    private float currentVelocityY;

    public ChaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.isAttacking = false;
        if (enemy is not FlyingEnemy)
        {
            enemy.OnEdgeDetected += StopAtEdge;
        }
        enemy.StateMachine.isChasing = true;
        //enemy.AttackScanner.ActivateScanner();
    }

    public void Update(float delta)
    {

    }

    public void FixedUpdate()
    {
        if (enemy.PlayerTransform != null && (enemy is FlyingEnemy || !enemy.IsEdgeDetected))
        {
            float dx = enemy.PlayerTransform.position.x - enemy.transform.position.x;
            float absDx = Mathf.Abs(dx);

            // 플레이어와 이 거리 이하로 가까워지면 더 이상 파고들지 않도록
            float stopDistance = 0.5f;

            // X축 이동 처리
            float targetVelocityX = 0f;
            if (absDx > stopDistance)
            {
                float dir = Mathf.Sign(dx);
                targetVelocityX = dir * enemy.EnemyStatus.TotalMoveSpeed;
                //enemy.Rigidbody.linearVelocity = new Vector2(dir * enemy.EnemyStatus.TotalMoveSpeed, enemy.Rigidbody.linearVelocity.y);
            }
            // --- 2. Y축 이동 처리 (지상 vs 공중 분기) ---
            float targetVelocityY = enemy.Rigidbody.linearVelocity.y; // 기본값

            if (enemy is FlyingEnemy flyingEnemy)
            {
                // FlyingEnemy에만 존재하는 targetYOffset과 smoothTime 사용
                float desiredY = enemy.PlayerTransform.position.y + flyingEnemy.targetYOffset;

                float smoothedY = Mathf.SmoothDamp(
                    enemy.transform.position.y,
                    desiredY,
                    ref currentVelocityY,
                    flyingEnemy.smoothTime
                );

                targetVelocityY = (smoothedY - enemy.transform.position.y) / Time.fixedDeltaTime;
            }

            enemy.Rigidbody.linearVelocity = new Vector2(targetVelocityX, targetVelocityY);
        }
        else
        {
            float stopY = enemy is FlyingEnemy ? 0f : enemy.Rigidbody.linearVelocity.y;
            enemy.Rigidbody.linearVelocity = new Vector2(0f, stopY);
        }

        enemy.EnemyAnimator.SetBool("isMove", enemy.Rigidbody.linearVelocity.magnitude > 0.1f);
    }


    public void Exit()
    {
        enemy.OnEdgeDetected -= StopAtEdge;
        enemy.Rigidbody.linearVelocity = Vector2.zero;
    }

    private void StopAtEdge()
    {
        // 플랫폼 끝에 도달했을 때 정지
        enemy.Rigidbody.linearVelocity = Vector2.zero;
    }
}
