using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : IState
{
    private Enemy enemy;
    private float movePos;
    private Vector2 direction;
    private float value;

    public WanderState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        movePos = Random.Range(enemy.LeftEdge.position.x, enemy.RightEdge.position.x);
        direction = (movePos > enemy.transform.position.x) ? Vector2.right : Vector2.left;
    }

    public void Update(float delta)
    {
        
    }

    public void FixedUpdate()
    {
        SetVelocity();
    }

    public void Exit()
    {
        enemy.Rigidbody.velocity = Vector2.zero;
        direction = Vector2.zero;
    }

    void SetVelocity()
    {
        if (direction.x >= 1f)
        {
            if (enemy.Rigidbody.position.x >= movePos)
            {
                enemy.Rigidbody.velocity = Vector2.zero;
                enemy.StateMachine.TransitionTo(enemy.StateMachine.idleState);
            }
            else
            {
                enemy.Rigidbody.velocity = direction * enemy.Speed;
            }
        }
        else if (direction.x <= -1f)
        {
            if (enemy.Rigidbody.position.x <= movePos)
            {
                enemy.Rigidbody.velocity = Vector2.zero;
                enemy.StateMachine.TransitionTo(enemy.StateMachine.idleState);
            }
            else
            {
                enemy.Rigidbody.velocity = direction * enemy.Speed;
            }
        }
    }
}
