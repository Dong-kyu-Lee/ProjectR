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
        Vector2 direction = (PlayerTransform.position - firePoint.position).normalized;
        float speed = 10f; // 원하는 발사 속도

        return direction * speed;
    }

    public IEnumerator EnableRangeAttack()
    {
        direction = SetTarget();
        yield return new WaitForSeconds(0.35f);
        ShootProjectile();
    }


    public override void ShootProjectile()
    {
        if (projectilePrefab && firePoint)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            RangedEnemyProjectile projScript = proj.GetComponent<RangedEnemyProjectile>();
            if (projScript != null)
            {
                projScript.enemy = this.gameObject;
                projScript.damage = EnemyStatus.Damage;
            }
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity = direction; // 투사체 속도
            }

            if (direction != Vector2.zero)
            {
                Quaternion baseRot = Quaternion.FromToRotation(Vector2.left, direction);

                Quaternion offset = Quaternion.Euler(0f, 0f, -20f); // 왼쪽 기준 아래로 20도

                proj.transform.rotation = baseRot * offset;
            }
        }
    }
}

