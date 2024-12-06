using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private Enemy enemy;
    float idleTime;

    public IdleState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        idleTime = Mathf.Round(Random.Range(1f, 2.5f) * 10f) / 10f;
        enemy.StateMachine.StartCoroutine(enemy.StateMachine.IdleCoroutine(idleTime));
    }

    public void Update(float delta)
    {

    }

    public void FixedUpdate()
    {

    }

    public void Exit()
    {
        enemy.StateMachine.StopCoroutine(enemy.StateMachine.IdleCoroutine(idleTime));
    }

    public IEnumerator IdleCoroutine(float idleTime)
    {
        yield return new WaitForSeconds(idleTime);
        enemy.StateMachine.TransitionTo(enemy.StateMachine.wanderState);
    }
}
