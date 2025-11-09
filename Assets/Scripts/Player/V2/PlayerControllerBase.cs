using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 모든 캐릭터의 고유 능력을 추상화하는 인터페이스
public interface IAbilityV2
{
    void Activate();
}

// 공통된 캐릭터 제어 로직을 담당
public abstract class PlayerControllerBase : MonoBehaviour
{
    protected Rigidbody2D playerRigidBody;
    [SerializeField]
    protected Animator playerAnimator;
    [SerializeField]
    protected Camera playerCamera;
    [SerializeField]
    protected GameObject rendererObject;
    protected PlayerStatus playerStatus;
    protected IAbilityV2 characterAbility; // 고유 능력 위임 객체

    public Transform groundCheck;
    public LayerMask groundLayer;
    protected float groundCheckRadius = 0.2f;

    public float jumpPower;
    public float moveFactor = 100f;
    public float dashFactor = 1f;
    public float dashTime = 0.2f;
    public float dashCoolTime = 0.5f;
    public float attackCoolTimeA = 0.5f;
    public float projectileSpawnOffset = 1f;

    protected bool enableJump = true;
    protected bool enableDash = true;
    protected bool enableAttack = true;
    protected bool isAttaking = false;
    protected bool isPause = false;
    protected bool isDead = false;
    protected bool isGround = false;

    private Vector2 _queuedAimDir;
    private bool _hasQueuedAim;

    protected bool isDashing = false;
    protected bool isInvincible = false;
    public bool IsInvincible => isInvincible;

    [Header("Dash Ghost")]
    [SerializeField] protected float dashGhostInterval = 0.03f; // 잔상 간격
    [SerializeField] private GameObject dashGhostTemplate;

    protected virtual void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();
        characterAbility = GetComponent<IAbilityV2>();
        CalcDamage.Instance.SetPlayer(this.gameObject);
        CalcReceiveDamage.Instance.SetPlayer(this.gameObject);
    }

    protected virtual void Update()
    {
        if (isPause || isDead) return;

        if (Input.GetKeyDown(KeyCode.Space) && enableJump)
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && enableDash && IsMovingHorizontally())
            StartCoroutine(Dash());

        if (Input.GetMouseButtonDown(0) && enableAttack)
        {
            // 1) 클릭 시 조준 벡터 계산
            var aim = GetAimDirection2D(horizontalOnly: false);

            // 2) 현재 바라보는 방향과 반대면 먼저 플립
            bool facingRight = rendererObject.GetComponent<SpriteRenderer>().flipX;
            bool aimRight = aim.x > 0f;
            if (aim.x != 0f && (aimRight != facingRight))
                Flip(aim.x);

            // 3) 공격 코루틴에서 같은 벡터를 쓰도록 큐에 저장
            _queuedAimDir = aim;
            _hasQueuedAim = true;

            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.Q))
            UseCharacterAbility();
    }

    protected virtual void FixedUpdate()
    {
        if (isPause || isDead) return;
        PlayerMove();
        JumpCheck();
    }

    // 좌우 이동 처리
    protected void PlayerMove()
    {
        Vector2 moveVector = new Vector2(
            Input.GetAxis("Horizontal") * playerStatus.TotalMoveSpeed * moveFactor * dashFactor * Time.deltaTime,
            playerRigidBody.velocity.y
        );
        playerRigidBody.velocity = moveVector;

        playerAnimator.SetBool("isMove", moveVector.x != 0f);
        if (moveVector.x != 0f && !isAttaking)
            Flip(moveVector.x);
    }

    // 방향 전환
    protected void Flip(float direction)
    {
        var sr = GetSprite();
        if (sr != null) sr.flipX = direction > 0;
    }

    // 점프 처리
    protected virtual void Jump()
    {
        enableJump = false;
        playerRigidBody.AddForce(new Vector2(0f, jumpPower));
    }

    // 지면 감지
    protected void JumpCheck()
    {
        enableJump = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        isGround = enableJump;

        playerAnimator.SetBool("isGround", isGround);
    }

    // 대시 처리
    protected virtual IEnumerator Dash()
    {
        if (isDashing || !enableDash || isDead || isPause || !IsMovingHorizontally())
            yield break;

        enableDash = false;
        isDashing = true;
        isInvincible = true;
        enableAttack = false; // 대시 중 공격 잠금 (원하면 빼도 됨)

        // 애니메이션 트리거 (각 플레이어 애니메이터에 "Dash" 트리거 추가)
        if (playerAnimator != null)
            playerAnimator.SetTrigger("dash");

        float originalDashFactor = dashFactor;
        dashFactor = 5f;

        // 잔상 생성 코루틴
        Coroutine ghostRoutine = null;
        if (dashGhostTemplate != null)
            ghostRoutine = StartCoroutine(DashGhostRoutine());

        // 실제 대시 지속 시간
        yield return new WaitForSeconds(dashTime);

        // 대시 종료
        dashFactor = originalDashFactor;
        isInvincible = false;
        isDashing = false;
        enableAttack = true;

        if (ghostRoutine != null)
            StopCoroutine(ghostRoutine);

        // 쿨타임
        yield return new WaitForSeconds(dashCoolTime);
        enableDash = true;
    }

    // 수평 이동 중인지 판정 (입력 or 실제 속도 기준)
    protected bool IsMovingHorizontally()
    {
        // 입력으로 먼저 판단
        float inputX = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(inputX) > 0.01f)
            return true;

        // 혹시 모를 외력(밀림 등)도 허용하고 싶으면 속도도 확인
        if (playerRigidBody != null && Mathf.Abs(playerRigidBody.velocity.x) > 0.01f)
            return true;

        return false;
    }


    protected virtual IEnumerator DashGhostRoutine()
    {
        while (isDashing)
        {
            SpawnDashGhost();
            yield return new WaitForSeconds(dashGhostInterval);
        }
    }

    protected void SpawnDashGhost()
    {
        if (dashGhostTemplate == null) return;

        var sr = GetSprite();
        if (sr == null || sr.sprite == null) return;

        // 템플릿을 기준으로 새 인스턴스 생성 (부모는 null로 해서 플레이어와 분리)
        var ghost = Instantiate(dashGhostTemplate, transform.position, Quaternion.identity);
        ghost.SetActive(true);

        var ghostSr = ghost.GetComponent<SpriteRenderer>();
        if (ghostSr != null)
        {
            ghostSr.sprite = sr.sprite;
            ghostSr.flipX = sr.flipX;
            ghostSr.sortingLayerID = sr.sortingLayerID;
            ghostSr.sortingOrder = sr.sortingOrder - 1;
        }
    }

    // 고유 능력 발동 위임
    protected virtual void UseCharacterAbility()
    {
        characterAbility?.Activate();
    }

    // 추상 공격 로직 - 하위 클래스에서 구체화
    protected abstract IEnumerator Attack();

    // 사망 처리
    public virtual void Dead()
    {
        StartCoroutine(DeadCoroutine());
    }

    protected virtual IEnumerator DeadCoroutine()
    {
        isDead = true;
        playerRigidBody.velocity = Vector2.zero;
        playerAnimator.SetTrigger("Die");
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.PlayerDead();
        //gameObject.SetActive(false);
    }

    public IAbilityV2 GetCharacterAbility()
    {
        return characterAbility;
    }

    protected Vector2 GetAimDirection2D(bool horizontalOnly = false)
    {
        var cam = GetActiveCamera();
        if (!cam)
        {
            // 카메라 정말 없으면 현재 바라보는 방향으로 보정
            var sr = GetSprite();
            float dir = (sr != null && sr.flipX) ? 1f : -1f;
            return new Vector2(dir, 0f);
        }

        Vector3 playerPos = transform.position;

        if (horizontalOnly)
        {
            float dx = Input.mousePosition.x - cam.WorldToScreenPoint(playerPos).x;
            return dx >= 0 ? Vector2.right : Vector2.left;
        }

        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane)
        );
        mouseWorld.z = playerPos.z;

        Vector2 dir2 = (Vector2)(mouseWorld - playerPos);
        if (dir2.sqrMagnitude < 1e-4f)
        {
            var sr = GetSprite();
            dir2 = (sr != null && sr.flipX) ? Vector2.right : Vector2.left;
        }
        return dir2.normalized;
    }

    protected Vector2 ConsumeQueuedAimDirection(bool horizontalOnly = false)
    {
        Vector2 dir;
        if (_hasQueuedAim)
        {
            dir = _queuedAimDir;
            _hasQueuedAim = false;
        }
        else
        {
            dir = GetAimDirection2D(horizontalOnly);
        }

        if (horizontalOnly)
        {
            float sx = dir.x;
            if (Mathf.Approximately(sx, 0f))
            {
                // 수평 전용인데 0이면 현재 바라보는 쪽으로 보정
                bool facingRight = rendererObject.GetComponent<SpriteRenderer>().flipX;
                sx = facingRight ? 1f : -1f;
            }
            dir = new Vector2(Mathf.Sign(sx), 0f);
        }

        if (dir.sqrMagnitude < 1e-6f)
        {
            bool facingRight = rendererObject.GetComponent<SpriteRenderer>().flipX;
            dir = facingRight ? Vector2.right : Vector2.left;
        }
        return dir.normalized;
    }

    protected Camera GetActiveCamera()
    {
        var cam = Camera.main;
        if (cam == null) cam = Camera.current;
        if (cam == null && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
        return cam;
    }

    protected SpriteRenderer GetSprite()
    {
        if (rendererObject == null) rendererObject = gameObject;
        var sr = rendererObject.GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        return sr;
    }

}
