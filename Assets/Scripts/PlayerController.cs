using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRigidBody;

    Vector2 moveVector = new Vector2(0f, 0f);

    PlayerStatus playerStatus;

    public UnityEvent OnEnableCharacterInfoUI;

    public GameObject projectilePref;
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
    public float attackCoolTime;
    public float projectielSpawnOffset;
    float minDistance = 0.5f;
    float groundCheckRadius = 0.2f;

    bool enableJump;
    bool enableDash;
    bool enableAttack;
    bool enableUI;
    bool isPause;

    void Start()
    {
        isPause = false;
        enableJump = true;
        enableDash = true;
        enableAttack = true;
        enableUI = true;
        dashFactor = 1.0f;
        dashTime = 0.2f;
        dashCoolTime = 1.0f;
        moveFactor = 100f;
        projectielSpawnOffset = 1f;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
        playerStatus = gameObject.GetComponent<PlayerStatus>();
        moveSpeed = playerStatus.MoveSpeed;
        attackCoolTime = playerStatus.TotalAttackSpeed;
    }

    void Update()
    {
        if (!isPause)
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
        if (!isPause)
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
    }

    // 점프
    void Jump()
    {
        if (enableJump)
        {
            enableJump = false;
            playerRigidBody.AddForce(new Vector2(0f, jumpPower));
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

        GameObject projectile = Instantiate(projectilePref, spawnPosition, Quaternion.identity);
        projectile.GetComponent<Projectile>().Velocity = direction;
        projectile.GetComponent<Projectile>().playerStatus = playerStatus;

        yield return new WaitForSeconds(attackCoolTime);
        enableAttack = true;
    }

    public void Dead()
    {
        StartCoroutine(DeadCoroutine());
    }

    IEnumerator DeadCoroutine()
    {
        playerAnimator.SetTrigger("Die");

        yield return new WaitForSeconds(1.5f);

        gameObject.SetActive(false);
    }
}
