using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : IState
{
    private Enemy enemy;

    public WanderState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("이동 상태 전환 완료.");
    }

    public void Update(float delta)
    {

    }

    public void Exit()
    {

    }
}
