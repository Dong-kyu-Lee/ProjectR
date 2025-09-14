using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float delay = 1.5f;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private string moneyType;
    [SerializeField] private float moneyAmount = 0f;

    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private Collider2D col;
    private Transform player;

    private bool isFlying = false;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        player = GameManager.Instance.CurrentPlayer.transform;
        StartCoroutine(LootDelay());
    }

    IEnumerator LootDelay()
    {
        yield return new WaitForSeconds(delay);
        isFlying = true;

        rigid.gravityScale = 0f;
        col.isTrigger = true;
    }

    private void Update()
    {
        if (isFlying && player != null)
        {
            // 플레이어 방향으로 이동
            transform.position = Vector3.SmoothDamp(
                transform.position,
                player.position,
                ref velocity,
                smoothTime
            );

            // 플레이어에게 닿으면 골드 획득 처리
            if (Vector3.Distance(transform.position, player.position) < stopDistance)
            {
                switch (moneyType)
                {
                    case "gold":
                        player.GetComponent<PlayerStatus>().Gold += moneyAmount;
                        break;
                    case "steadfite":
                        player.GetComponent<PlayerStatus>().Steadfite += (int)moneyAmount;
                        break;
                    default:
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}
