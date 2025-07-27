using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D projectileRigidbody;
    public PlayerStatus playerStatus;
    public BartenderAbility bartenderAbility;
    public GameObject player;
    public float damage;
    public float ignoreDamageReduction;
    public string bottle;
    private bool hasExploded = false;

    public GameObject explosionPrefab;
    protected float explosionRadius = 5f;

    Vector2 velocity;

    // 발사 속도 프로퍼티
    public Vector2 Velocity
    {
        get
        {
            return velocity;
        }
        set
        {
            velocity = value;
            projectileRigidbody.velocity = velocity * 50f;
        }
    }

    void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;
        hasExploded = true;

        float damage = playerStatus.TotalDamage;
        float ignoreDamageReduction = playerStatus.IgnoreDamageReduction;
        bool isCritical = false;

        if (AbilityManager.Instance.bartenderAbility2)
        {
            Vector3 hitPoint = transform.position;

            Instantiate(explosionPrefab, hitPoint, Quaternion.identity);

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hitPoint, explosionRadius);

            HashSet<GameObject> processedEnemies = new HashSet<GameObject>();

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    GameObject enemyObject = hitCollider.gameObject;

                    EnemyStatus enemy = hitCollider.GetComponent<EnemyStatus>();
                    if (enemy != null && !processedEnemies.Contains(enemyObject))
                    {
                        processedEnemies.Add(enemyObject);
                        float thisdamage = CalcDamage.Instance.CheckCritical(damage / 2, ref ignoreDamageReduction, ref isCritical);
                        enemy.gameObject.GetComponent<Status>().TakeDamage(player, thisdamage, ignoreDamageReduction, isCritical);
                        bartenderAbility.BartenderAttackDebuff(enemyObject);
                        bartenderAbility.CheckBartenderAbility(bottle);
                        CalcDamage.Instance.CheckAddtionalDamage(enemyObject);
                        CalcDamage.Instance.AdditionalEffect(enemyObject);
                        CalcDamage.Instance.CheckFightState();
                    }
                }
            }
        }

        if (collision.transform.tag == "Enemy")
        {
            float thisdamage = CalcDamage.Instance.CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
            collision.gameObject.GetComponent<Status>().TakeDamage(player, thisdamage, ignoreDamageReduction, isCritical);
            bartenderAbility.BartenderAttackDebuff(collision.gameObject);
            bartenderAbility.CheckBartenderAbility(bottle);
            CalcDamage.Instance.CheckAddtionalDamage(collision.gameObject);
            CalcDamage.Instance.AdditionalEffect(collision.gameObject);
            CalcDamage.Instance.CheckFightState();
        }
        gameObject.SetActive(false);
    }
}
