using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected EnemyStatus enemyStatus;

    [SerializeField]
    protected Animator enemyAnimator;

    [SerializeField]
    private BoxCollider2D attackRangeCol;

    [SerializeField]
    protected BoxCollider2D chaseRangeCol;

    [SerializeField]
    protected EnemyAIController enemyController;

    [SerializeField]
    protected Transform playerTransform;

    public EnemyAIController StateMachine { get { return enemyController; } }
    public BoxCollider2D AttackRangeCol { get { return attackRangeCol; } }
    public BoxCollider2D ChaseRangeCol { get { return chaseRangeCol; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
