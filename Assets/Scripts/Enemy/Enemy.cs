using System;
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
    protected AttackScanner attackScanner;

    [SerializeField]
    protected BoxCollider2D chaseRangeCol;

    [SerializeField]
    protected EnemyAIController enemyController;

    [SerializeField]
    protected Transform playerTransform;

    [SerializeField]
    protected Rigidbody2D enemyRigidbody;

    [SerializeField]
    protected float speed = 3f;

    [SerializeField]
    private Transform groundCheckPoint;

    [SerializeField]
    private float groundCheckDistance = 1f;

    [SerializeField]
    private LayerMask groundLayer;

    public bool isAttacking = false;

    public event Action OnEdgeDetected;

    public float deadDelayTime = 1.5f;

    public EnemyStatus EnemyStatus { get { return enemyStatus; } }
    public EnemyAIController StateMachine { get { return enemyController; } }
    public AttackScanner AttackScanner { get { return attackScanner; } }
    public BoxCollider2D ChaseRangeCol { get { return chaseRangeCol; } }
    public Transform PlayerTransform { get { return playerTransform; } }
    public float Speed { get { return speed; } }
    public Rigidbody2D Rigidbody { get { return enemyRigidbody; } }
    public Animator EnemyAnimator { get { return enemyAnimator; } }

    public event Action<Enemy> onDeath;

    public bool IsEdgeDetected { get; private set; }

    protected IAttackStrategy attackStrategy;

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
        CheckPlatformEdge();
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
        if (!StateMachine.isDead && !isAttacking)
        {
            StateMachine.TransitionTo(StateMachine.attackState);
            Attack();
        }
    }

    public virtual void Attack()
    {
        FacePlayer();
        //attackStrategy?.ExecuteAttack(this);
    }

    protected void CheckPlatformEdge()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);

        Debug.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckDistance, isGroundAhead ? Color.green : Color.red);

        if (!isGroundAhead)
        {
            IsEdgeDetected = true;
            OnEdgeDetected?.Invoke();
        }
        else
        {
            IsEdgeDetected = false;
        }
    }

    public void Dead()
    {
        CalcDamage.Instance.KillEnemyBuff();
        StartCoroutine(DeadCoroutine());
        onDeath?.Invoke(this);
    }

    protected IEnumerator DeadCoroutine()
    {
        yield return new WaitForSeconds(deadDelayTime);
        gameObject.SetActive(false);
    }

    public virtual void SetAttackStrategy(IAttackStrategy strategy)
    {
        attackStrategy = strategy;
    }

    public void FacePlayer()
    {
        if (PlayerTransform == null) return;

        float dir = PlayerTransform.position.x - transform.position.x;
        if (dir > 0f)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (dir < 0f)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public virtual void PerformAttack()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Attack");
        }
    }

    public virtual void ShootProjectile()
    {

    }
}
