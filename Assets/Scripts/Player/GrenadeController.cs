using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private GameObject grenadePrefab;

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

    // [추가] 안전하게 현재 활성화된 카메라를 가져오는 함수
    private Camera GetActiveCamera()
    {
        if (Camera.main != null) return Camera.main;
        if (Camera.allCamerasCount > 0) return Camera.allCameras[0];
        return null;
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
        Camera cam = GetActiveCamera();
        if (cam == null) return Vector2.zero; // 카메라가 없으면 힘을 0으로 반환하여 에러 방지

        // 마우스 입력 위치 계산
        Vector3 mouseInput = Input.mousePosition;
        mouseInput.z = Mathf.Abs(cam.transform.position.z);
        Vector3 mousePos = cam.ScreenToWorldPoint(mouseInput);

        // 마우스 방향 계산
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