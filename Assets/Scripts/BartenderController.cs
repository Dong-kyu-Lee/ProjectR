using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BartenderController : MonoBehaviour
{
    Rigidbody2D playerRigidBody;

    Vector2 moveVector = new Vector2(0f, 0f);

    PlayerStatus playerStatus;

    public UnityEvent OnEnableCharacterInfoUI;

    public GameObject projectilePref;
    public GameObject rendererObject;
    public Camera playerCamera;
    public Animator playerAnimator;
    public LayerMask groundLayer;
    public Transform groundCheck;


    float moveSpeed;
    public float jumpPower;
    public float moveFactor;
    public float dashFactor;
    public float dashTime;
    public float dashCoolTime;
    public float attackCoolTimeA;
    public float attackCoolTimeB;
    public float projectielSpawnOffset;
    float minDistance = 0.5f;
    float groundCheckRadius = 0.2f;

    bool enableJump;
    bool enableDash;
    bool enableAttack;
    bool enableUI;
    bool isPause;
    bool isAttaking;
    bool isDead;

    void Start()
    {
        isPause = false;
        enableJump = true;
        enableDash = true;
        enableAttack = true;
        enableUI = true;
        isAttaking = false;
        isDead = false;
        dashFactor = 1.0f;
        dashTime = 0.2f;
        dashCoolTime = 1.0f;
        moveFactor = 100f;
        projectielSpawnOffset = 1f;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
        playerStatus = gameObject.GetComponent<PlayerStatus>();
        moveSpeed = playerStatus.MoveSpeed;
        attackCoolTimeA = 0.5f;
        attackCoolTimeB = playerStatus.TotalAttackSpeed - attackCoolTimeA;
    }

    void Update()
    {
        if (!isPause && !isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space) && enableJump)
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (enableDash)
                {
                    StartCoroutine(Dash());
                }
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                EnableCharacterUI();
            }

            if (Input.GetMouseButtonDown(0) && enableAttack)
            {
                StartCoroutine(Attack());
            }
        }

    }

    private void FixedUpdate()
    {
        if (!isPause && !isDead)
        {
            PlayerMove();
            JumpCheck();
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // 플레이어 좌우 이동 속도 지정
    void PlayerMove()
    {
        moveVector.x = Input.GetAxis("Horizontal") * moveSpeed * moveFactor * dashFactor * Time.deltaTime;
        moveVector.y = playerRigidBody.velocity.y;
        playerRigidBody.velocity = moveVector;
        if (playerRigidBody.velocity.x != 0f)
        {
            playerAnimator.SetBool("isMove", true);
            if (!isAttaking)
            {
                Flip();
            }
        }
        else
        {
            playerAnimator.SetBool("isMove", false);

        }
    }

    // 플레이어 좌우 이동에 따른 y축 회전
    void Flip()
    {
        if (playerRigidBody.velocity.x > 0)
        {
            rendererObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (playerRigidBody.velocity.x < 0)
        {
            rendererObject.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    // 점프
    void Jump()
    {
        if (enableJump)
        {
            enableJump = false;
            playerRigidBody.AddForce(new Vector2(0f, jumpPower));
            playerAnimator.SetTrigger("jump");
        }
    }

    void JumpCheck()
    {
        // 플레이어 아래에 발판 오브젝트가 오버랩되는지 확인
        enableJump = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // 캐릭터 정보 UI 활성화
    void EnableCharacterUI()
    {
        if (enableUI)
        {
            isPause = true;
            enableUI = false;
            OnEnableCharacterInfoUI.Invoke();
        }
    }

    // 캐릭터 정보 UI 비활성화
    public void DisableCharacterUI()
    {
        isPause = false;
        enableUI = true;
    }

    // 대쉬
    IEnumerator Dash()
    {
        Debug.Log("Dash couroutine on");
        if (enableDash)
        {
            enableDash = false;
            dashFactor = 5.0f;

            yield return new WaitForSeconds(dashTime);
            dashFactor = 1.0f;

            yield return new WaitForSeconds(dashCoolTime);
            enableDash = true;
        }
    }

    // 공격
    IEnumerator Attack()
    {
        enableAttack = false;
        isAttaking = true;
        playerAnimator.SetTrigger("Attack");

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(playerCamera.transform.position.z);
        Vector3 curMousePoint = playerCamera.ScreenToWorldPoint(mousePos);

        Vector3 direction = curMousePoint - gameObject.transform.position;
        float distance = direction.magnitude;
        Debug.Log(distance);

        if (distance < minDistance)
        {
            direction = (direction.x > 0 ? Vector3.right : Vector3.left);
        }
        else
        {
            direction.Normalize();
        }

        Vector3 spawnPosition = gameObject.transform.position + direction * projectielSpawnOffset;

        if (spawnPosition.x > gameObject.transform.position.x)
        {
            rendererObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            rendererObject.GetComponent<SpriteRenderer>().flipX = false;
        }

        GameObject projectile = Instantiate(projectilePref, spawnPosition, Quaternion.identity);
        projectile.GetComponent<Projectile>().playerStatus = playerStatus;
        projectile.GetComponent<Projectile>().Velocity = direction;
        projectile.GetComponent<Projectile>().damage = playerStatus.Damage;

        yield return new WaitForSeconds(attackCoolTimeA);
        isAttaking = false;

        yield return new WaitForSeconds(attackCoolTimeB);
        enableAttack = true;
    }

    public void Dead()
    {
        StartCoroutine(DeadCoroutine());
    }

    IEnumerator DeadCoroutine()
    {
        isDead = true;
        playerRigidBody.velocity = Vector2.zero;
        playerAnimator.SetTrigger("Die");

        yield return new WaitForSeconds(1.5f);

        gameObject.SetActive(false);
    }

    // 공격의 크리티컬 여부 확인.
    public float CheckCritical()
    {
        float damage = playerStatus.TotalDamage;
        float criticalPercnet = playerStatus.CriticalPercent;
        float criticalDamage = playerStatus.CriticalDamage;

        float randomValue = Random.Range(0f, 100f);

        if (randomValue < criticalPercnet)
        {
            Debug.Log("크리티컬!");
            return damage * (1 + criticalDamage * 0.01f);
        }
        else
        {
            return damage;
        }
    }
}
