using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// xMoveAmount, yMoveAmount 만큼 움직이고 다시 원래 위치로 돌아오는 클래스
public class MovingTile : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("X축 이동량")] public float xMoveAmount = 2f;
    [Tooltip("Y축 이동량")] public float yMoveAmount = 1f;
    [Tooltip("이동 속도 (유닛/초)")] public float speed = 1f;

    private Vector3 originalPos;
    private Vector3 targetPos;
    private Vector3 currentTarget;
    private Rigidbody2D rb2D;

    void Start()
    {
        originalPos = transform.position;
        targetPos = originalPos + new Vector3(xMoveAmount, yMoveAmount, 0);
        currentTarget = targetPos;

        rb2D = GetComponent<Rigidbody2D>();
        if(rb2D != null) rb2D.isKinematic = true;
    }

    /*void Update()
    {
        // 물리 미사용 시
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            speed * Time.deltaTime
        );

        CheckTargetUpdate();
    }*/


    // 물리 사용 시 (FixedUpdate에서 처리)
    void FixedUpdate()
    {
        if (rb2D != null)
        {
            Vector3 newPos = Vector3.MoveTowards(
                rb2D.position,
                currentTarget,
                speed * Time.fixedDeltaTime
            );
            rb2D.MovePosition(newPos);
        }
        CheckTargetUpdate();
    }


    void CheckTargetUpdate()
    {
        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            currentTarget = (currentTarget == targetPos) ? originalPos : targetPos;
        }
    }
}
