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
            Vector2 chaseDirection = (enemy.PlayerTransform.position.x > enemy.transform.position.x) ? Vector2.right : Vector2.left;
            enemy.Rigidbody.velocity = new Vector2(chaseDirection.x * enemy.Speed, enemy.Rigidbody.velocity.y);
        }
        else
        {
            enemy.Rigidbody.velocity = Vector2.zero;
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
