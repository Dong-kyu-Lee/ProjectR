using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : IState
{
    private Enemy enemy;
    private float movePos;
    private Vector2 direction;
    private Rigidbody2D rigidbody;
    private float value;
    private float speed = 10f;

    public WanderState(Enemy enemy)
    {
        this.enemy = enemy;
        rigidbody = enemy.GetComponent<Rigidbody2D>();
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

    }

    void SetVelocity()
    {
        if (direction.x >= 1f)
        {
            if (rigidbody.position.x >= movePos)
            {
                rigidbody.velocity = Vector2.zero;
                enemy.StateMachine.TransitionTo(enemy.StateMachine.idleState);
            }
            else
            {
                rigidbody.velocity = direction * speed;
            }
        }
        else if (direction.x <= -1f)
        {
            if (rigidbody.position.x <= movePos)
            {
                rigidbody.velocity = Vector2.zero;
                enemy.StateMachine.TransitionTo(enemy.StateMachine.idleState);
            }
            else
            {
                rigidbody.velocity = direction * speed;
            }
        }
    }
}
