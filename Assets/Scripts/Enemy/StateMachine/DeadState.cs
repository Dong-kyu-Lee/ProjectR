using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Enemy enemy;

    public DeadState(Enemy enemy)
    {
        this.enemy = enemy;

    }

    public void Enter()
    {
        enemy.EnemyAnimator.SetTrigger("Die");
        enemy.Dead();
        enemy.StateMachine.isDead = true;
    }

    public void Update(float delta)
    {

    }

    public void FixedUpdate()
    {

    }

    public void Exit()
    {

    }
}
