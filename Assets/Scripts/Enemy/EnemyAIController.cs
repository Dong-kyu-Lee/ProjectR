using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public IState CurrentState { get; private set; }

    public IdleState idleState;
    public WanderState wanderState;
    public ChaseState chaseState;
    public AttackState attackState;

    public void Initialize(Enemy enemy)
    {
        this.idleState = new IdleState(enemy);
        this.wanderState = new WanderState(enemy);
        this.chaseState = new ChaseState(enemy);
        this.attackState = new AttackState(enemy);

        CurrentState = idleState;
        idleState.Enter();
    }

    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.FixedUpdate();
        }
    }

    public IEnumerator IdleCoroutine(float idleTime)
    {
        yield return new WaitForSeconds(idleTime);
        TransitionTo(wanderState);
    }
}
