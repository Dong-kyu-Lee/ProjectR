using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

    private Vector2 velocity;

    // 투사체 이동 속도 배율
    [SerializeField] private float speedMultiplier = 15f;
    // 포물선 궤적을 위해 Rigidbody2D의 중력 비율
    [SerializeField] private float gravityScale = 2f;
    // 초기 상승 속도. 수치가 클수록 더 높은 호를 형성
    [SerializeField] private float initialUpwardVelocity = 1.2f;

    // 발사 속도 설정 (Velocity 속성으로 외부에서 설정 
    public Vector2 Velocity
    {
        get => velocity;
        set
        {
            // 중력 적용
            projectileRigidbody.gravityScale = gravityScale;
            // 수평 속도는 방향 벡터의 x성분에 speedMultiplier를 곱해 지정
            // 수직 속도는 initialUpwardVelocity만큼 추가하여 포물선 궤적 형성.
            projectileRigidbody.velocity = new Vector2(value.x * speedMultiplier,
                                                       value.y * speedMultiplier + initialUpwardVelocity);
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