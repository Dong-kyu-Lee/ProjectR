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

    [SerializeField]
    protected float speed = 3f;

    public EnemyAIController StateMachine { get { return enemyController; } }
    public BoxCollider2D AttackRangeCol { get { return attackRangeCol; } }
    public BoxCollider2D ChaseRangeCol { get { return chaseRangeCol; } }
    public Transform LeftEdge { get { return leftEdge; } }
    public Transform RightEdge { get { return rightEdge; } }
    public Transform PlayerTransform { get { return playerTransform; } }
    public float Speed { get { return speed; } }
    public Rigidbody2D Rigidbody { get { return enemyRigidbody; } }
    public Animator EnemyAnimator { get { return enemyAnimator; } }

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
        CheckEdge();
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

    public void SetTarget(Transform transform)
    {
        playerTransform = transform;
        StateMachine.isChasing = true;
        StateMachine.TransitionTo(StateMachine.chaseState);
    }

    public void StartAttack()
    {
        StateMachine.TransitionTo(StateMachine.attackState);
        Attack();
    }

    protected virtual void Attack()
    {

    }

    protected void CheckEdge()
    {
        if (transform.position.x >= RightEdge.transform.position.x)
        {
            transform.position = new Vector3(RightEdge.transform.position.x, transform.position.y, transform.position.z);
        }
        else if(transform.position.x <= LeftEdge.transform.position.x)
        {
            transform.position = new Vector3(LeftEdge.transform.position.x, transform.position.y, transform.position.z);
        }
    }

    public void Dead()
    {
        CalcDamage.Instance.KillEnemyBuff();
        StartCoroutine(DeadCoroutine());
    }

    protected IEnumerator DeadCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}
