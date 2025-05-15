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
        enemy.StateMachine.isChasing = true;
    }

    public void Update(float delta)
    {

    }

    public void FixedUpdate()
    {
        if (enemy.PlayerTransform != null)
        {
            enemy.Rigidbody.velocity = (enemy.PlayerTransform.position.x > enemy.transform.position.x) ? Vector2.right : Vector2.left;
            enemy.Rigidbody.velocity *= enemy.EnemyStatus.TotalMoveSpeed;
        }
    }

    public void Exit()
    {
        enemy.Rigidbody.velocity = Vector2.zero;
    }
}
