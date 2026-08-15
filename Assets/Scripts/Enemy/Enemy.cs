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

    [SerializeField]
    private float hitStunDurationOnInterrupt = 0.15f;

    [SerializeField]
    private float attackRetryDelayAfterInterrupt = 0.6f;

    [SerializeField]
    private EnemyAttackProfile attackProfile;

    [SerializeField]
    private bool hasSuperArmor;

    [SerializeField]
    private float maxPoise = 1f;

    [SerializeField]
    private float poiseDamagePerHit = 1f;

    [SerializeField]
    private float poiseRecoveryDelay = 1.5f;

    [SerializeField]
    private float poiseRecoveryPerSecond = 1f;

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

    public EnemyAttackPhase CurrentAttackPhase { get; private set; } = EnemyAttackPhase.None;
    public bool CanInterruptCurrentAttack { get; private set; }

    private float nextAttackAllowedTime;
    private float currentPoise;
    private float lastPoiseDamageTime = float.NegativeInfinity;

    protected virtual void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        currentPoise = MaxPoise;
    }

    void Start()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        FlipX();
        CheckPlatformEdge();
        RecoverPoise();
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
        // 적이 죽었거나, 이미 공격 중이거나, 현재 경직(Stun) 상태라면 공격 실행 금지
        if (StateMachine.isDead || isAttacking || StateMachine.CurrentState == StateMachine.stunState)
        {
            return;
        }

        if (Time.time < nextAttackAllowedTime)
        {
            return;
        }

        StateMachine.TransitionTo(StateMachine.attackState);
        Attack();
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

    public EnemyAttackTiming GetAttackTiming(IAttackStrategy strategy)
    {
        if (attackProfile != null)
        {
            return attackProfile.Timing;
        }

        return strategy.Timing;
    }

    public void SetAttackPhase(EnemyAttackPhase phase, bool canInterrupt)
    {
        CurrentAttackPhase = phase;
        CanInterruptCurrentAttack = canInterrupt;
    }

    public bool TryInterruptAttack()
    {
        return TryInterruptAttack(HitStunDurationOnInterrupt, PoiseDamagePerHit);
    }

    public bool TryInterruptAttack(float stunDuration)
    {
        return TryInterruptAttack(stunDuration, PoiseDamagePerHit);
    }

    public bool TryInterruptAttack(float stunDuration, float poiseDamage)
    {
        if (enemyStatus != null && enemyStatus.IsBoss)
        {
            return false;
        }

        if (HasSuperArmor)
        {
            return false;
        }

        if (StateMachine == null)
        {
            return false;
        }

        if (StateMachine.CurrentState != StateMachine.attackState)
        {
            return false;
        }

        if (!CanInterruptCurrentAttack)
        {
            return false;
        }

        if (!ConsumePoise(poiseDamage))
        {
            return false;
        }

        ResetPoise();
        DelayNextAttack(AttackRetryDelayAfterInterrupt);
        ApplyHitStun(stunDuration);
        return true;
    }

    public void DelayNextAttack(float delay)
    {
        nextAttackAllowedTime = Mathf.Max(nextAttackAllowedTime, Time.time + delay);
    }

    private bool ConsumePoise(float poiseDamage)
    {
        lastPoiseDamageTime = Time.time;
        currentPoise -= Mathf.Max(0f, poiseDamage);

        return currentPoise <= 0f;
    }

    private void ResetPoise()
    {
        currentPoise = MaxPoise;
    }

    private void RecoverPoise()
    {
        float targetPoise = MaxPoise;

        if (currentPoise >= targetPoise) return;
        if (Time.time < lastPoiseDamageTime + PoiseRecoveryDelay) return;

        currentPoise = Mathf.Min(targetPoise, currentPoise + PoiseRecoveryPerSecond * Time.fixedDeltaTime);
    }

    private float HitStunDurationOnInterrupt
    {
        get { return attackProfile != null ? attackProfile.HitStunDurationOnInterrupt : Mathf.Max(0f, hitStunDurationOnInterrupt); }
    }

    private float AttackRetryDelayAfterInterrupt
    {
        get { return attackProfile != null ? attackProfile.AttackRetryDelayAfterInterrupt : Mathf.Max(0f, attackRetryDelayAfterInterrupt); }
    }

    private bool HasSuperArmor
    {
        get { return attackProfile != null ? attackProfile.HasSuperArmor : hasSuperArmor; }
    }

    private float MaxPoise
    {
        get { return attackProfile != null ? attackProfile.MaxPoise : Mathf.Max(1f, maxPoise); }
    }

    private float PoiseDamagePerHit
    {
        get { return attackProfile != null ? attackProfile.PoiseDamagePerHit : Mathf.Max(0f, poiseDamagePerHit); }
    }

    private float PoiseRecoveryDelay
    {
        get { return attackProfile != null ? attackProfile.PoiseRecoveryDelay : Mathf.Max(0f, poiseRecoveryDelay); }
    }

    private float PoiseRecoveryPerSecond
    {
        get { return attackProfile != null ? attackProfile.PoiseRecoveryPerSecond : Mathf.Max(0f, poiseRecoveryPerSecond); }
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

    public void ApplyHitStun(float duration)
    {
        if (enemyStatus != null && (enemyStatus.IsBoss || enemyStatus.Hp <= 0)) return;

        // 실행 중이던 공격 판정(Melee의 Hitbox 대기 등) 코루틴 강제 정지
        StopAllCoroutines();

        // AI 상태 머신 내부의 대기(Idle, Attack Delay) 코루틴 정지
        if (StateMachine != null) StateMachine.StopAllCoroutines();

        // 애니메이터에 큐(Queue)로 들어가 대기 중인 공격 트리거를 캔슬
        if (enemyAnimator != null)
        {
            enemyAnimator.ResetTrigger("Attack");
        }

        // 켜져있는 무기 히트박스 등을 끔 (CancelAttack은 이전 답변에서 만든 가상 함수)
        CancelAttack();
        SetAttackPhase(EnemyAttackPhase.None, false);

        // StunState로 경직 시간 전달 후 상태 강제 전환!
        if (StateMachine != null && StateMachine.stunState is StunState stun)
        {
            stun.SetDuration(duration);
            StateMachine.TransitionTo(StateMachine.stunState);
        }
    }

    public virtual void CancelAttack()
    {
        isAttacking = false;
        SetAttackPhase(EnemyAttackPhase.None, false);
    }
}
