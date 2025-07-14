using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        StateMachine.Initialize(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void ShootProjectile()
    {
        if (projectilePrefab && firePoint)
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
}

