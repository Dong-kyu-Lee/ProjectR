using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemyProjectile : MonoBehaviour
{
    public float damage;
    public float lifeTime = 1.5f;
    private bool isDamaged = false;

    [SerializeField]
    private LayerMask targetLayer;

    public GameObject enemy;

    void Awake()
    {
        isDamaged = false;
    }

    void Start()
    {
        // 일정 시간 후 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("GroundTile"))
        {
            Destroy(gameObject);
        }

        // 지정 레이어 감지
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            // PlayerStatus 컴포넌트에 데미지 전달
            var playerStatus = collision.gameObject.GetComponent<PlayerStatus>();
            if (playerStatus != null && !isDamaged)
            {
                collision.gameObject.GetComponent<Status>().TakeDamage(enemy, damage, 0, false);
                isDamaged = true;
            }

            // 충돌 후 파괴
            Destroy(gameObject);
        }
    }
}
