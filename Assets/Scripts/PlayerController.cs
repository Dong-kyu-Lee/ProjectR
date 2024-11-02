using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRigidBody;

    Vector2 moveVector = new Vector2(0f, 0f);

    public GameObject projectilePref;
    public Camera playerCamera;

    public float moveSpeed;
    public float jumpPower;
    public float dashFactor;
    public float dashTime;
    public float dashCoolTime;
    public float attackCoolTime;

    bool enableJump;
    bool enableDash;
    bool enableAttack;

    void Start()
    {
        enableJump = true;
        enableDash = true;
        enableAttack = true;
        dashFactor = 1.0f;
        dashTime = 0.2f;
        dashCoolTime = 1.0f;
        attackCoolTime = 0.5f;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMove();

        if (Input.GetKeyDown(KeyCode.Space))
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

        if (Input.GetMouseButtonDown(0) && enableAttack)
        {
            StartCoroutine(Attack());
        }
    }

    // 플레이어 좌우 이동 속도 지정
    void PlayerMove()
    {
        moveVector.x = Input.GetAxis("Horizontal") * moveSpeed * dashFactor * Time.deltaTime;
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

        Vector3 curMouseVector = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 spawnVector;
        if (curMouseVector.x >= transform.position.x)
        {
            spawnVector = new Vector2(transform.position.x + 1.5f, transform.position.y);
        }
        else
        {
            spawnVector = new Vector2(transform.position.x - 1.5f, transform.position.y);
        }

        GameObject projectileObj = Instantiate(projectilePref, spawnVector, Quaternion.identity);
        Vector2 velocityVector = new Vector2(curMouseVector.x - spawnVector.x, curMouseVector.y - spawnVector.y);
        projectileObj.GetComponent<Projectile>().Velocity = velocityVector.normalized;

        yield return new WaitForSeconds(attackCoolTime);
        enableAttack = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥 충돌 시 내적계산을 통해 점프 가능 변수 활성화
        Vector2 hitNormalVector = collision.GetContact(0).normal;

        if (Mathf.Approximately(hitNormalVector.y, 1.0f))
        {
            float dotResult = Vector2.Dot(gameObject.transform.right, hitNormalVector);

            if (Mathf.Approximately(dotResult, 0.0f))
            {
                enableJump = true;
            }
        }
    }
}
