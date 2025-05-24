using System.Collections;
using System.Collections.Generic;
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
    public float dashCoolTime = 1f;
    public float attackCoolTimeA = 0.5f;
    public float projectileSpawnOffset = 1f;

    protected bool enableJump = true;
    protected bool enableDash = true;
    protected bool enableAttack = true;
    protected bool isAttaking = false;
    protected bool isPause = false;
    protected bool isDead = false;
    protected bool isGround = false;

    protected virtual void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();
        characterAbility = GetComponent<IAbilityV2>();
    }

    protected virtual void Update()
    {
        if (isPause || isDead) return;

        if (Input.GetKeyDown(KeyCode.Space) && enableJump)
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && enableDash)
            StartCoroutine(Dash());

        if (Input.GetMouseButtonDown(0) && enableAttack)
            StartCoroutine(Attack());

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
        rendererObject.GetComponent<SpriteRenderer>().flipX = direction > 0;
    }

    // 점프 처리
    protected virtual void Jump()
    {
        enableJump = false;
        playerRigidBody.AddForce(new Vector2(0f, jumpPower));
        playerAnimator.SetTrigger("jump");
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
        enableDash = false;
        dashFactor = 5f;
        yield return new WaitForSeconds(dashTime);
        dashFactor = 1f;
        yield return new WaitForSeconds(dashCoolTime);
        enableDash = true;
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
        gameObject.SetActive(false);
    }
}
