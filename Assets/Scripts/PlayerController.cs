using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRigidBody;
    Vector2 moveVector = new Vector2(0f, 0f);

    public float moveSpeed;
    public float jumpPower;

    bool isJumping;

    void Start()
    {
        isJumping = false;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMove();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    // 플레이어 좌우 이동 속도 지정
    void PlayerMove()
    {
        moveVector.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        moveVector.y = playerRigidBody.velocity.y;
        playerRigidBody.velocity = moveVector;
    }

    // 점프 수행
    void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            playerRigidBody.AddForce(new Vector2(0f, jumpPower));
        }
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
                isJumping = false;
            }
        }
    }
}
