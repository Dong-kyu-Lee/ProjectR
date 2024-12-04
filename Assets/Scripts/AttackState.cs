using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private Enemy enemy;

    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;

    }

    public void Enter()
    {

    }

    public void Update(float delta)
    {

    }

    public void Exit()
    {

    }
}
