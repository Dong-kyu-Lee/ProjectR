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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 벽/땅 충돌 처리 (레이어 이름 확인)
        if (collision.gameObject.layer == LayerMask.NameToLayer("GroundTile"))
        {
            Destroy(gameObject);
            return;
        }

        PlayerControllerBase pcb = collision.gameObject.GetComponent<PlayerControllerBase>();

        // 2. 플레이어 무적(대쉬 등)일 때 처리
        if (pcb != null && pcb.IsInvincible)
        {
            return;
        }

        // 3. 데미지 처리 (Target Layer 확인)
        // 비트 연산으로 레이어 체크
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            var playerStatus = collision.gameObject.GetComponent<PlayerStatus>();

            if (playerStatus != null && !isDamaged)
            {
                // 데미지 적용
                collision.gameObject.GetComponent<Status>().TakeDamage(enemy, damage, 0, false);
                isDamaged = true;
            }

            // 데미지를 줬으면 파괴
            Destroy(gameObject);
        }
    }
}
