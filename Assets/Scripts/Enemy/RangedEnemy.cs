using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    private Vector2 direction;

    protected override void Awake()
    {
        base.Awake();
        SetAttackStrategy(new RangedAttackStrategy());
    }

    void Start()
    {
        StateMachine.Initialize(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public Vector2 SetTarget()
    {
        Vector2 direction = PlayerTransform.position - firePoint.position;

        float distance = direction.magnitude;
        float radAngle = 30f * Mathf.Deg2Rad; // 30도

        // 속력 계산 공식 (포물선 방정식 기반)
        float velocity = Mathf.Sqrt(distance * 9.8f / Mathf.Sin(2 * radAngle));

        // x, y 성분 분리
        float vx = velocity * Mathf.Cos(radAngle);
        float vy = velocity * Mathf.Sin(radAngle);

        // 방향 보정 (좌우 반전)
        if (PlayerTransform.position.x < firePoint.position.x)
            vx = -vx;

        return new Vector2(vx, vy);
    }

    public IEnumerator EnableRangeAttack()
    {
        direction = SetTarget();
        yield return new WaitForSeconds(0.3f);
        ShootProjectile();
    }


    public override void ShootProjectile()
    {
        if (projectilePrefab && firePoint)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            proj.GetComponent<RangedEnemyProjectile>().enemy = this.gameObject;
            proj.GetComponent<RangedEnemyProjectile>().damage = enemyStatus.Damage;
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity = direction; // 투사체 속도
            }
        }
    }
}

