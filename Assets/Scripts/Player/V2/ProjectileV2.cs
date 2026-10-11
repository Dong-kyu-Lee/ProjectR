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

    public GameObject[] explosionPrefabs = new GameObject[4];
    public float explosionRadius;
    public LayerMask enemyLayer;

    [Header("Throw")]
    [SerializeField, Min(0f)] private float speedMultiplier = 18f;
    [SerializeField, Min(0f)] private float gravityScale = 2f;
    [SerializeField] private float initialUpwardVelocity = 4f;
    [SerializeField, Min(0.1f)] private float lifetime = 3f;

    public float Lifetime => lifetime;
    public Vector2 Gravity => Physics2D.gravity * gravityScale;

    // 기존 호출부는 방향을 전달한다. 읽을 때는 실제 물리 속도를 반환한다.
    public Vector2 Velocity
    {
        get => projectileRigidbody.velocity;
        set
        {
            projectileRigidbody.gravityScale = gravityScale;
            projectileRigidbody.velocity = GetLaunchVelocity(value);
        }
    }

    // 미리보기와 실제 투척이 같은 초기 속도를 사용한다.
    public Vector2 GetLaunchVelocity(Vector2 direction)
    {
        return direction.normalized * speedMultiplier + Vector2.up * initialUpwardVelocity;
    }

    public static Vector2 GetSecondThrowDirection(Vector2 direction)
    {
        return (direction.normalized + Vector2.up * 0.2f).normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;
        hasExploded = true;

        float damage = playerStatus.TotalDamage;
        float ignoreDamageReduction = playerStatus.IgnoreDamageReduction;
        bool isCritical = false;

        Vector3 hitPoint = transform.position;
        HashSet<GameObject> processedEnemies = new HashSet<GameObject>();

        GameObject explosionPrefab;
        switch (bottle)
        {
            case "Burn":
                explosionPrefab = explosionPrefabs[1];
                break;
            case "Poison":
                explosionPrefab = explosionPrefabs[2];
                break;
            case "Freeze":
                explosionPrefab = explosionPrefabs[3];
                break;
            default:
                explosionPrefab = explosionPrefabs[0];
                break;
        }
        // 닿은 곳에 술병 폭발 생성
        GameObject explosion =Instantiate(explosionPrefab, hitPoint, Quaternion.identity);
        int explosionScale = 1;
        if (AbilityManager.Instance.bartenderAbility[1]) explosionScale = 3;
        explosion.transform.localScale = explosionPrefab.transform.localScale * explosionScale;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hitPoint, explosionRadius * explosionScale);

        foreach (var hitCollider in hitColliders)
        {
            EnemyStatus enemyStatus = hitCollider.GetComponentInParent<EnemyStatus>();
            if (enemyStatus != null)
            {
                if (hitCollider.gameObject != enemyStatus.gameObject) continue;

                GameObject enemy = enemyStatus.gameObject;

                float explodeDamage = CalcDamage.Instance.CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
                enemyStatus?.TakeDamage(player, explodeDamage, ignoreDamageReduction, isCritical);
                processedEnemies.Add(enemy);

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

        // 일반 충돌
        if (collision.transform.CompareTag("Enemy") && !processedEnemies.Contains(collision.gameObject))
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
