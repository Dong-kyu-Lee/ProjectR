using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralGrenade : Grenade
{
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float explodeDamage = 100f;
    [SerializeField]
    private float explosionRadius = 6f;

    // 폭발 생성.
    protected override void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hitCollider in hitColliders)
        {
            EnemyStatus enemyStatus = hitCollider.GetComponentInParent<EnemyStatus>();
            if (enemyStatus != null)
            {
                if (hitCollider.gameObject != enemyStatus.gameObject) continue;

                GameObject enemy = enemyStatus.gameObject;

                enemyStatus?.TakeDamage(this.gameObject, explodeDamage, 0, false);
                CalcDamage.Instance.CheckFightState();
            }
        }
        Destroy(gameObject);
    }
}
