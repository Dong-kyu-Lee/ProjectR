using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable Item", menuName = "Item/Throwable")]
public class ThrowableItemData : ConsumableItemData
{
    [Header("던질 때 적용될 효과")]
    public float explosionRadius = 2.0f;
    public float damage = 30f;
    public GameObject explosionEffectPrefab;

    public override void ActivateItemEffect(PlayerStatus player)
    {
        Debug.Log("슛");
    }

    // 실제 투척 시 호출될 메서드
    public void Throw(Vector3 origin)
    {
        // 폭발 이펙트 생성
        if (explosionEffectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(explosionEffectPrefab, origin, Quaternion.identity);
            GameObject.Destroy(effect, 2f);
        }

        // 범위 안의 적에게 피해 적용
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(origin, explosionRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in hitEnemies)
        {
            // 적에게 데미지를 주는 로직
            var status = enemy.GetComponent<Status>();
            if (status != null)
                status.Hp -= damage;
        }
    }
}
