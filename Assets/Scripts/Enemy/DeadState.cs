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
