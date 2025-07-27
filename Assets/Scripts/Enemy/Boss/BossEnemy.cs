using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhaseType
{
    Phase1,
    Phase2,
    Phase3
}

public class BossEnemy : Enemy
{
    protected PhaseType currentPhase = PhaseType.Phase1;

    public float debuffDuration = 3f;
    public float debuffAmount = 0.5f;

    [SerializeField] private GameObject meleeHitBox;
    [SerializeField] private GameObject debuffHitBox;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    public GameObject MeleeHitBox => meleeHitBox;
    public GameObject DebuffHitBox => debuffHitBox;
    public GameObject ProjectilePrefab => projectilePrefab;
    public Transform FirePoint => firePoint;

    protected override void Awake()
    {
        base.Awake();
        SetAttackStrategy(new MeleeAttackStrategy());
        AttackRangeCol.size = new Vector2(enemyStatus.EnemyStatusData.AttackRange, AttackRangeCol.size.y);
    }

    void Start()
    {
        StateMachine.Initialize(this);
    }

    protected virtual void Update()
    {
        CheckPhaseChange();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void CheckPhaseChange()
    {
        float hpRate = EnemyStatus.Hp / EnemyStatus.MaxHp;

        switch (currentPhase)
        {
            case PhaseType.Phase1:
                if (hpRate <= 0.7f)
                {
                    currentPhase = PhaseType.Phase2;
                    OnPhase2Start();
                }
                break;
            case PhaseType.Phase2:
                if (hpRate <= 0.4f)
                {
                    currentPhase = PhaseType.Phase3;
                    OnPhase3Start();
                }
                break;
        }
    }

    protected virtual void OnPhase2Start()
    {
        Debug.Log("Phase 2 시작");
        EnemyStatus.MoveSpeed *= 1.5f;
        SetAttackStrategy(new DebuffAttackStrategy());
    }

    protected virtual void OnPhase3Start()
    {
        Debug.Log("Phase 3 시작");
        var strategies = new List<IAttackStrategy>()
        {
            new MeleeAttackStrategy(),
            new RangedAttackStrategy(),
            new DebuffAttackStrategy()
        };

        SetAttackStrategy(new RandomCompositeStrategy(this, strategies));
    }

    public override void ShootProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.GetComponent<RangedEnemyProjectile>().enemy = this.gameObject;
        proj.GetComponent<RangedEnemyProjectile>().damage = enemyStatus.Damage;
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb)
        {
            Vector2 dir = (PlayerTransform.position - firePoint.position).normalized;
            rb.velocity = dir * 20f; // 투사체 속도
        }
    }

}
