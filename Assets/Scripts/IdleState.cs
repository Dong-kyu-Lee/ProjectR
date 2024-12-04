using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private Enemy enemy;
    float idleTime;
    float time;

    public IdleState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        idleTime = Mathf.Round(Random.Range(1f, 2.5f) * 10f) / 10f;
        time = 0f;
        Debug.Log("Idle State. 대기 시간 : " + idleTime);
    }

    public void Update(float delta)
    {
        time += delta;
        if (time >= idleTime)
        {
            Debug.Log("시간 : " + time + "상태 전환");
            enemy.StateMachine.TransitionTo(enemy.StateMachine.wanderState);
        }
    }

    public void Exit()
    {

    }


}
