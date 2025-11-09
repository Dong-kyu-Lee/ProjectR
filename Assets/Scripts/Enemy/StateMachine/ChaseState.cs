using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private Enemy enemy;

    public ChaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.isAttacking = false;
        enemy.OnEdgeDetected += StopAtEdge;
        enemy.StateMachine.isChasing = true;
        enemy.AttackScanner.ActivateScanner();
    }

    public void Update(float delta)
    {

    }

    public void FixedUpdate()
    {
        if (enemy.PlayerTransform != null && !enemy.IsEdgeDetected)
        {
            float dx = enemy.PlayerTransform.position.x - enemy.transform.position.x;
            float absDx = Mathf.Abs(dx);

            // 플레이어와 이 거리 이하로 가까워지면 더 이상 파고들지 않도록
            float stopDistance = 0.5f;

            if (absDx > stopDistance)
            {
                float dir = Mathf.Sign(dx);
                enemy.Rigidbody.velocity = new Vector2(dir * enemy.Speed, enemy.Rigidbody.velocity.y);
            }
            else
            {
                // 사정거리 안에 들어오면 제자리 유지(공격 상태로 전환은 AttackRange 트리거가 담당)
                enemy.Rigidbody.velocity = new Vector2(0f, enemy.Rigidbody.velocity.y);
            }
        }
        else
        {
            enemy.Rigidbody.velocity = new Vector2(0f, enemy.Rigidbody.velocity.y);
        }

        enemy.EnemyAnimator.SetBool("isMove", enemy.Rigidbody.velocity.magnitude > 0.1f);
    }


    public void Exit()
    {
        enemy.OnEdgeDetected -= StopAtEdge;
        enemy.Rigidbody.velocity = Vector2.zero;
    }

    private void StopAtEdge()
    {
        // 플랫폼 끝에 도달했을 때 정지
        enemy.Rigidbody.velocity = Vector2.zero;
    }
}
