using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject grenadePrefab;

    public int resolution = 10;
    public float launchForce = 15f;
    public float gravity = -9.81f;

    private bool isAiming = false;

    void Update()
    {
        // 우클릭 떼면 발사
        if (Input.GetMouseButtonUp(1) && isAiming)
        {
            LaunchGrenade();
            isAiming = false;
            lineRenderer.enabled = false;
        }

        // 조준 중 궤적 업데이트
        if (isAiming)
        {
            DrawTrajectory();
        }
    }

    // 수류탄 조준
    public void StartAim(GameObject grenade)
    {
        isAiming = true;
        lineRenderer.enabled = true;
        grenadePrefab = grenade;
    }

    // 조준선 그리기
    private void DrawTrajectory()
    {
        Vector2 startPos = transform.position;
        Vector2 velocity = CalculateVelocity();

        lineRenderer.positionCount = resolution;
        float gravity = Physics2D.gravity.y * 1f; // Rigidbody2D의 Gravity Scale이 1인 경우

        for (int i = 0; i < resolution; i++)
        {
            float time = i * 0.015f;
            float x = startPos.x + velocity.x * time;
            float y = startPos.y + (velocity.y * time) + (0.5f * gravity * time * time);

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    // 속도 계산
    private Vector2 CalculateVelocity()
    {
        // 2D 마우스 위치 계산
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = ((Vector2)mousePos - (Vector2)transform.position).normalized;

        // 마우스 방향으로 힘을 가함
        return direction * launchForce;
    }

    // 수류탄 발사
    private void LaunchGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        rb.velocity = CalculateVelocity();
    }
}
