using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 투사체. 적과 충돌 시 데미지를 입히고, 바텐더 전용 병 효과를 적용한다.
public class ProjectileV2 : MonoBehaviour
{
    public Rigidbody2D projectileRigidbody;
    public PlayerStatus playerStatus;

    public BartenderAbilityV2 bartenderAbility;  // ← BartenderAbility → V2 타입으로 변경
    public GameObject player;

    public float damage;
    public float ignoreDamageReduction;
    public string bottle;

    private bool hasExploded = false;

    public GameObject explosionPrefab;
    public float explosionRadius = 5f;

    private Vector2 velocity;

    // 발사 속도 설정 (Velocity 속성으로 외부에서 설정 
    public Vector2 Velocity
    {
        get => velocity;
        set
        {
            velocity = value;
            projectileRigidbody.velocity = velocity * 50f;
        }
    }

    private void Start()
    {
        Destroy(gameObject, 1f); // 수명 제한
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;
        hasExploded = true;

        float damage = playerStatus.TotalDamage;
        float ignoreDamageReduction = playerStatus.IgnoreDamageReduction;
        bool isCritical = false;

        Vector3 hitPoint = transform.position;

        // 광역 폭발 기능
        if (AbilityManager.Instance.bartenderAbility[1])
        {
            Instantiate(explosionPrefab, hitPoint, Quaternion.identity);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hitPoint, explosionRadius);
            HashSet<GameObject> processedEnemies = new HashSet<GameObject>();

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    GameObject enemy = hitCollider.gameObject;

                    if (processedEnemies.Contains(enemy)) continue;
                    processedEnemies.Add(enemy);

                    damage = CalcDamage.Instance.CheckCritical(damage, ref ignoreDamageReduction, ref isCritical) / 2;
                    enemy.GetComponent<Status>()?.TakeDamage(player, damage, ignoreDamageReduction, isCritical);

                    if (bartenderAbility != null)
                    {
                        bartenderAbility.BartenderAttackDebuff(enemy);
                        bartenderAbility.CheckBartenderAbility(enemy, bottle);
                    }

                    CalcDamage.Instance.CheckAddtionalDamage(enemy);
                    CalcDamage.Instance.AdditionalEffect(enemy);
                    CalcDamage.Instance.CheckFightState();
                }
            }
        }

        // 일반 충돌
        if (collision.transform.CompareTag("Enemy"))
        {
            damage = CalcDamage.Instance.CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
            collision.gameObject.GetComponent<Status>()?.TakeDamage(player, damage, ignoreDamageReduction, isCritical);

            if (bartenderAbility != null)
            {
                bartenderAbility.BartenderAttackDebuff(collision.gameObject);
                bartenderAbility.CheckBartenderAbility(collision.gameObject, bottle);
            }

            CalcDamage.Instance.CheckAddtionalDamage(collision.gameObject);
            CalcDamage.Instance.AdditionalEffect(collision.gameObject);
            CalcDamage.Instance.CheckFightState();
        }

        gameObject.SetActive(false);
    }

}