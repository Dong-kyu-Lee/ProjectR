using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public IState CurrentState { get; private set; }

    public IdleState idleState;

    public void Initialize(Enemy enemy)
    {
        this.idleState = new IdleState(enemy);

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
        CurrentState.Update();
    }
}
