using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 캐릭터의 고유 능력을 추상화하는 인터페이스
public interface IAbilityV2
{
    void Activate();
    void ResetAbility();
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
    [SerializeField]
    protected GrenadeController GrenadeController;
    public string playerName;
    protected PlayerStatus playerStatus;
    protected IAbilityV2 characterAbility; // 고유 능력 위임 객체

    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.7f, 0.15f);

    public float jumpPower;

    [Header("Jump Feel")]
    [SerializeField, Range(0.1f, 1f)] private float jumpCutMultiplier = 0.5f;
    [SerializeField, Min(1f)] private float fallGravityMultiplier = 1.5f;
    [SerializeField, Min(0f)] private float coyoteTime = 0.1f;
    [SerializeField, Min(0f)] private float jumpBufferTime = 0.1f;

    [Header("Drop Through Platform")]
    [SerializeField, Min(0f)] private float dropThroughDuration = 0.2f;
    [SerializeField, Min(0f)] private float dropThroughSpeed = 2f;
    public float moveFactor = 100f;
    public float dashFactor = 1f;
    public float dashTime = 0.2f;
    public float dashCoolTime = 0.5f;
    public float attackCoolTimeA = 0.767f;
    public float projectileSpawnOffset = 1f;

    protected bool enableJump = true;
    protected bool enableDash = true;
    protected bool enableAttack = true;
    protected bool isAttaking = false;
    protected bool isPause = false;
    protected bool isDead = false;
    protected bool isGround = false;

    private float lastGroundedTime = float.NegativeInfinity;
    private float lastJumpPressedTime = float.NegativeInfinity;
    private Collider2D currentGroundCollider;
    private Coroutine dropThroughCoroutine;

    private struct IgnoredCollision
    {
        public Collider2D PlayerCollider;
        public Collider2D PlatformCollider;

        public IgnoredCollision(Collider2D playerCollider, Collider2D platformCollider)
        {
            PlayerCollider = playerCollider;
            PlatformCollider = platformCollider;
        }
    }

    private readonly List<IgnoredCollision> ignoredDropThroughCollisions = new List<IgnoredCollision>();

    private Vector2 _queuedAimDir;
    private bool _hasQueuedAim;

    protected bool isDashing = false;
    protected bool isInvincible = false;
    public bool IsInvincible => isInvincible;

    [Header("Dash Ghost")]
    [SerializeField] protected float dashGhostInterval = 0.03f; // 잔상 간격
    [SerializeField] private GameObject dashGhostTemplate;

    [Header("Effects")]
    [SerializeField] protected GameObject dustPrefab;       
    [SerializeField] protected float dustInterval = 0.3f;   // 먼지 생성 간격
    private float dustTimer;

    protected bool IsMovementDisabled => playerStatus != null && playerStatus.TotalMoveSpeed <= 0f;

    protected virtual void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();
        characterAbility = GetComponent<IAbilityV2>();
    }

    protected virtual void Update()
    {
        if (isPause || isDead) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 이동 불가 상태일 때는 하향 점프 및 일반 점프 입력 무시
            if (!IsMovementDisabled)
            {
                if (IsPressingDown() && TryStartDropThrough())
                {
                    lastJumpPressedTime = float.NegativeInfinity;
                }
                else
                {
                    lastJumpPressedTime = Time.time;
                    TryConsumeBufferedJump();
                }
            }
        }

        // 상승 중 점프 키를 놓으면 상승 속도를 잘라 짧은 점프를 만든다.
        if (Input.GetKeyUp(KeyCode.Space) && playerRigidBody.linearVelocity.y > 0f)
        {
            Vector2 velocity = playerRigidBody.linearVelocity;
            velocity.y *= jumpCutMultiplier;
            playerRigidBody.linearVelocity = velocity;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && enableDash && !IsMovementDisabled && IsMovingHorizontally())
            StartCoroutine(Dash());

        bool isUIActive = InGameUIManager.Instance != null && InGameUIManager.Instance.IsUIActive;
        HandleAttackInput(isUIActive);

        if (Input.GetKeyDown(KeyCode.Q))
            UseCharacterAbility();
    }

    // 기본 캐릭터는 누르는 순간 공격하고, 바텐더는 입력 처리를 재정의한다.
    protected virtual void HandleAttackInput(bool isUIActive)
    {
        if (Input.GetMouseButtonDown(0) && enableAttack && !isDashing && !isUIActive)
        {
            StartAimedAttack(GetAimDirection2D(horizontalOnly: false));
        }
    }

    protected void StartAimedAttack(Vector2 aim)
    {
        if (aim.x != 0f) Flip(aim.x);
        _queuedAimDir = aim;
        _hasQueuedAim = true;
        StartCoroutine(Attack());
    }

    protected virtual void FixedUpdate()
    {
        if (isPause || isDead) return;
        PlayerMove();
        JumpCheck();
        TryConsumeBufferedJump();
        ApplyAdditionalFallGravity();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    // 좌우 이동 처리
    protected void PlayerMove()
    {
        float inputX = Input.GetAxis("Horizontal");

        // 이동속도가 0 이하인 경우 이동 불가능
        if (IsMovementDisabled)
        {
            inputX = 0f;
        }

        Vector2 moveVector = new Vector2(
            inputX * playerStatus.TotalMoveSpeed * moveFactor * dashFactor * Time.deltaTime,
            playerRigidBody.linearVelocity.y
        );
        playerRigidBody.linearVelocity = moveVector;


        bool isMoving = !IsMovementDisabled && Mathf.Abs(inputX) > 0.01f;
        playerAnimator.SetBool("isMove", isMoving);

        if (isMoving && isGround && !isDashing)
        {
            dustTimer -= Time.deltaTime;
            if (dustTimer <= 0)
            {
                SpawnDust();
                dustTimer = dustInterval;
            }
        }

        if (moveVector.x != 0f && !isAttaking)
            Flip(moveVector.x);
    }

    protected void SpawnDust()
    {
        if (dustPrefab == null || groundCheck == null) return;

        // groundCheck 위치(발밑)에 먼지 생성
        Instantiate(dustPrefab, groundCheck.position, Quaternion.identity);
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
        if (IsMovementDisabled) return;

        enableJump = false;
        isGround = false;

        // 기존 AddForce(jumpPower)의 결과를 유지하되, 프레임에 힘이 누적되는 대신
        // 계산된 초기 속도를 직접 지정해 점프 높이가 안정적으로 나오게 한다.
        float jumpVelocity = jumpPower * Time.fixedDeltaTime / playerRigidBody.mass;
        playerRigidBody.linearVelocity = new Vector2(playerRigidBody.linearVelocity.x, jumpVelocity);

        if (playerAnimator != null)
            playerAnimator.SetBool("isGround", false);
    }

    // 지면 감지
    protected void JumpCheck()
    {
        // 캐릭터 발 너비에 가까운 얕은 박스로 검사해 발판 가장자리에서도
        // 실제 Collider가 지지되는 동안 지상 상태를 유지한다.
        currentGroundCollider = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );
        bool isHittingGround = currentGroundCollider != null && dropThroughCoroutine == null;

        if (playerRigidBody.linearVelocity.y > 0.05f)
        {
            isHittingGround = false;
        }

        enableJump = isHittingGround && !IsMovementDisabled;
        isGround = enableJump;

        if (isGround)
            lastGroundedTime = Time.time;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isGround", isGround);
        }
    }

    private void TryConsumeBufferedJump()
    {
        // 이동 불가 상태일 때는 버퍼 점프 실행 차단
        if (IsMovementDisabled) return;

        bool hasBufferedInput = Time.time - lastJumpPressedTime <= jumpBufferTime;
        bool isWithinCoyoteTime = Time.time - lastGroundedTime <= coyoteTime;

        if (!hasBufferedInput || (!enableJump && !isWithinCoyoteTime) || dropThroughCoroutine != null)
            return;

        lastJumpPressedTime = float.NegativeInfinity;
        lastGroundedTime = float.NegativeInfinity;
        Jump();
    }

    private void ApplyAdditionalFallGravity()
    {
        if (playerRigidBody.linearVelocity.y >= 0f || fallGravityMultiplier <= 1f)
            return;

        Vector2 extraGravity = Physics2D.gravity
            * playerRigidBody.gravityScale
            * (fallGravityMultiplier - 1f)
            * Time.fixedDeltaTime;
        playerRigidBody.linearVelocity += extraGravity;
    }

    private bool IsPressingDown()
    {
        return Input.GetKey(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") < -0.5f;
    }

    private bool TryStartDropThrough()
    {
        if (IsMovementDisabled || !isGround || currentGroundCollider == null || !IsOneWayPlatform(currentGroundCollider))
            return false;

        if (dropThroughCoroutine != null)
            StopCoroutine(dropThroughCoroutine);

        dropThroughCoroutine = StartCoroutine(DropThroughPlatform(currentGroundCollider));
        return true;
    }

    private bool IsOneWayPlatform(Collider2D platformCollider)
    {
        if (platformCollider.GetComponent<PlatformEffector2D>() != null)
            return true;

        Rigidbody2D attachedBody = platformCollider.attachedRigidbody;
        return attachedBody != null && attachedBody.GetComponent<PlatformEffector2D>() != null;
    }

    private IEnumerator DropThroughPlatform(Collider2D platformCollider)
    {
        RestoreDropThroughCollisions();

        Collider2D[] playerColliders = GetComponents<Collider2D>();
        foreach (Collider2D playerCollider in playerColliders)
        {
            if (!playerCollider.enabled || playerCollider.isTrigger)
                continue;

            Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
            ignoredDropThroughCollisions.Add(new IgnoredCollision(playerCollider, platformCollider));
        }

        enableJump = false;
        isGround = false;
        currentGroundCollider = null;
        lastGroundedTime = float.NegativeInfinity;

        Vector2 velocity = playerRigidBody.linearVelocity;
        velocity.y = Mathf.Min(velocity.y, -dropThroughSpeed);
        playerRigidBody.linearVelocity = velocity;

        if (playerAnimator != null)
            playerAnimator.SetBool("isGround", false);

        yield return new WaitForSeconds(dropThroughDuration);

        RestoreDropThroughCollisions();
        dropThroughCoroutine = null;
    }

    private void RestoreDropThroughCollisions()
    {
        foreach (IgnoredCollision ignoredCollision in ignoredDropThroughCollisions)
        {
            if (ignoredCollision.PlayerCollider != null && ignoredCollision.PlatformCollider != null)
            {
                Physics2D.IgnoreCollision(
                    ignoredCollision.PlayerCollider,
                    ignoredCollision.PlatformCollider,
                    false
                );
            }
        }

        ignoredDropThroughCollisions.Clear();
    }

    // 대시 처리
    protected virtual IEnumerator Dash()
    {
        if (isDashing || !enableDash || isDead || isPause || IsMovementDisabled || !IsMovingHorizontally())
            yield break;

        enableDash = false;
        isDashing = true;
        isInvincible = true;

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

        if (ghostRoutine != null)
            StopCoroutine(ghostRoutine);

        // 쿨타임
        yield return new WaitForSeconds(dashCoolTime);
        enableDash = true;
    }

    // 수평 이동 중인지 판정 (입력 or 실제 속도 기준)
    protected bool IsMovingHorizontally()
    {
        if (IsMovementDisabled) return false;

        // 입력으로 먼저 판단
        float inputX = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(inputX) > 0.01f)
            return true;

        // 혹시 모를 외력(밀림 등)도 허용하고 싶으면 속도도 확인
        if (playerRigidBody != null && Mathf.Abs(playerRigidBody.linearVelocity.x) > 0.01f)
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
        if (isDead) return;

        isDead = true;

        StopAllCoroutines();
        StartCoroutine(DeadCoroutine());
    }

    protected virtual IEnumerator DeadCoroutine()
    {
        playerRigidBody.linearVelocity = Vector2.zero;

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Attack");
            playerAnimator.ResetTrigger("dash");
            playerAnimator.SetBool("isMove", false);
            playerAnimator.SetBool("isGround", false);

            playerAnimator.SetTrigger("Die");
        }

        yield return new WaitForSeconds(1.5f);

        PlayerManager.Instance.PlayerDead();
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
        Camera mainCamera = Camera.main;
        if (IsAimCamera(mainCamera)) return mainCamera;
        if (IsAimCamera(playerCamera)) return playerCamera;

        // Camera.current는 렌더링용 카메라이므로 입력 좌표 변환에 사용하지 않는다.
        // 태그가 없는 테스트 씬에서도 미니맵/Scene View/비활성 카메라는 제외한다.
        Camera fallback = null;
        foreach (Camera candidate in Camera.allCameras)
        {
            if (!IsAimCamera(candidate)) continue;
            if (fallback == null || candidate.depth > fallback.depth)
                fallback = candidate;
        }
        return fallback;
    }

    private bool IsAimCamera(Camera candidate)
    {
        return candidate != null && candidate.isActiveAndEnabled
            && candidate.cameraType == CameraType.Game
            && candidate.targetTexture == null
            && (candidate.cullingMask & (1 << gameObject.layer)) != 0;
    }

    protected SpriteRenderer GetSprite()
    {
        if (rendererObject == null) rendererObject = gameObject;
        var sr = rendererObject.GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        return sr;
    }

    protected virtual void OnEnable()
    {
        ResetPlayerState();
    }

    protected virtual void OnDisable()
    {
        RestoreDropThroughCollisions();
        dropThroughCoroutine = null;
        StopAllCoroutines();

        // 물리 속도 즉시 정지 (관성 제거)
        if (playerRigidBody != null)
            playerRigidBody.linearVelocity = Vector2.zero;
    }

    public virtual void ResetPlayerState()
    {
        if (dropThroughCoroutine != null)
        {
            StopCoroutine(dropThroughCoroutine);
            dropThroughCoroutine = null;
        }
        RestoreDropThroughCollisions();

        // 제어 변수 초기화
        enableJump = true;
        enableDash = true;
        enableAttack = true;

        // 상태 변수 초기화
        isAttaking = false;
        isDashing = false;
        isInvincible = false;
        isPause = false;
        isGround = false;
        currentGroundCollider = null;
        lastGroundedTime = float.NegativeInfinity;
        lastJumpPressedTime = float.NegativeInfinity;
        _hasQueuedAim = false;

        // 대시 중에 비활성화 되었을 경우 계수를 원래대로 복구
        dashFactor = 1f;

        // 물리 초기화
        if (playerRigidBody != null)
            playerRigidBody.linearVelocity = Vector2.zero;

        // 애니메이터 초기화 (남아있는 트리거 제거)
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Attack");
            playerAnimator.ResetTrigger("dash");
            playerAnimator.SetBool("isMove", false);
            playerAnimator.SetBool("isGround", false);

            if (!isDead)
                playerAnimator.ResetTrigger("Die");
        }

        // 모든 스킬의 쿨타임을 초기화하여 씬 전환/재시작 시 버그 방지
        if (CalcDamage.Instance != null)
        {
            CalcDamage.Instance.ResetAllCooldowns(); // 내부 쿨타임 전체 초기화
        }

        // 모든 캐릭터 공통으로 스킬 쿨타임 UI(그림자)도 초기화
        if (InGameUIManager.Instance != null && InGameUIManager.Instance.skillCoolTimeUI != null)
        {
            InGameUIManager.Instance.skillCoolTimeUI.ResetCooldownUI();
        }
    }
}
