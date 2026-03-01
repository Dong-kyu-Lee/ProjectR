using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMo : MonoBehaviour
{
    Rigidbody2D playerRigidBody;

    Vector2 moveVector = new Vector2(0f, 0f);

    public float moveSpeed;
    public float jumpPower;
    public float dashFactor;
    public float dashTime;
    public float dashCoolTime;
    public float attackCoolTime;


    void Start()
    {
        dashFactor = 1.0f;
        dashTime = 0.2f;
        dashCoolTime = 1.0f;
        attackCoolTime = 0.5f;
        moveSpeed = 500.0f;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }
    void PlayerMove()
    {
        moveVector.x = Input.GetAxis("Horizontal") * moveSpeed * dashFactor * Time.deltaTime;
        moveVector.y = playerRigidBody.velocity.y;
        playerRigidBody.velocity = moveVector;
    }
}
