using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FlyingEnemy : Enemy
{
    [Header("Flying Settings")]
    public float targetYOffset = 2f;
    public float smoothTime = 0.3f;

    private Collider2D myCollider;
    private Rigidbody2D rb;
    private bool isFallingToDeath = false;
    private bool isDeadLanded = false;

    [SerializeField]
    public GameObject hitBoxObj;

    protected override void Awake()
    {
        base.Awake();
        myCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f; // 평소 비행 시 중력 0
        }

        int flyingLayer = gameObject.layer;
        int floatingTileLayer = LayerMask.NameToLayer("FloatingTile");

        if (floatingTileLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(flyingLayer, floatingTileLayer, true);
        }
    }

    void Start()
    {
        StateMachine.Initialize(this);
    }

    public override void Dead()
    {
        if (isFallingToDeath || (StateMachine != null && StateMachine.isDead)) return;

        isFallingToDeath = true;

        // 1. 공격 판정 및 히트박스 끄기
        if (hitBoxObj != null) hitBoxObj.SetActive(false);
        if (attackScanner != null) attackScanner.gameObject.SetActive(false);

        // 2. AI 상태 머신 정지
        if (StateMachine != null)
        {
            StateMachine.enabled = false;
        }

        // 3. Rigidbody를 오직 물리(중력)에만 맡기기
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.5f;
        }

        StartCoroutine(FallbackDeathTimerCoroutine(5f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDeadLanded) return;

        // 낙하 중일 때
        if (isFallingToDeath)
        {
            if ((1 << collision.gameObject.layer & LayerMask.GetMask("GroundTile")) != 0)
            {
                TriggerLandDeath();
            }
        }
    }

    // 바닥에 충돌 시 사망 트리거 개시
    private void TriggerLandDeath()
    {
        if (isDeadLanded) return;
        isDeadLanded = true;
        isFallingToDeath = false;

        // 콜라이더 위치 이동
        float liftAmount = -2f;
        myCollider.offset = new Vector2(myCollider.offset.x, myCollider.offset.y + liftAmount);

        StartCoroutine(DeadCoroutine());
        OnDeath();

        // 사망 애니메이션 재생
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("LandDead");
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (myCollider != null)
        {
            myCollider.enabled = false;
        }
    }

    private IEnumerator FallbackDeathTimerCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDeadLanded)
        {
            TriggerLandDeath();
        }
    }
}
