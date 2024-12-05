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

    [SerializeField]
    protected Transform leftEdge;

    [SerializeField]
    protected Transform rightEdge;

    [SerializeField]
    protected Rigidbody2D enemyRigidbody;

    public EnemyAIController StateMachine { get { return enemyController; } }
    public BoxCollider2D AttackRangeCol { get { return attackRangeCol; } }
    public BoxCollider2D ChaseRangeCol { get { return chaseRangeCol; } }
    public Transform LeftEdge { get { return leftEdge; } }
    public Transform RightEdge { get { return rightEdge; } }

    protected virtual void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        FlipX();
    }

    protected void FlipX()
    {
        if(enemyRigidbody.velocity.x >= 1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if(enemyRigidbody.velocity.x <= -1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
