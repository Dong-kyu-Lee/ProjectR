using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public IState CurrentState { get; private set; }

    public IState idleState;
    public IState wanderState;
    public IState chaseState;
    public IState attackState;
    public IState deadState;

    public bool isChasing = false;
    public bool isDead;

    public void Initialize(Enemy enemy)
    {
        this.idleState = new IdleState(enemy);
        this.wanderState = new WanderState(enemy);
        this.chaseState = new ChaseState(enemy);
        this.deadState = new DeadState(enemy);

        if (enemy is RangedEnemy rangedEnemy)
        {
            this.attackState = new RangedAttackState(rangedEnemy);
        }
        else
        {
            this.attackState = new MeleeAttackState(enemy);
        }

        isChasing = false;
        isDead = false;

        CurrentState = idleState;
        idleState.Enter();
    }

    public void TransitionTo(IState nextState)
    {
        if (!isDead)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            nextState.Enter();
        }
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
        if (isChasing == false)
            TransitionTo(wanderState);
    }

    public IEnumerator AttackCoroutine(float attackTime)
    {
        yield return new WaitForSeconds(attackTime);
        TransitionTo(chaseState);
    }

    public void StartChase()
    {
        isChasing = true;
    }
}
