using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// 로비 씬에 사용되는 시네머신을 관리하는 스크립트
public class CM_LobbyScene : MonoBehaviour
{
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;

    [Header("Zoom Settings")]
    [SerializeField] private float defaultOrthoSize = 5f; // 평소 로비 카메라 사이즈 (기존 사이즈에 맞춰 수정 필요)
    [SerializeField] private float zoomOrthoSize = 3f;  // 캐릭터 선택 시 줌인 될 사이즈
    [SerializeField] private float zoomSpeed = 3f;      // 줌인/아웃 속도

    private Coroutine zoomCoroutine;

    void Awake()
    {
        virtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    public void SetFollowTarget(Transform target)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = target;
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera not found on this GameObject.");
        }
    }

    public void SetZoom(bool isZoomIn)
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }

        float targetSize = isZoomIn ? zoomOrthoSize : defaultOrthoSize;
        zoomCoroutine = StartCoroutine(SmoothZoom(targetSize));
    }

    private IEnumerator SmoothZoom(float targetSize)
    {
        if (virtualCamera == null) yield break;

        while (Mathf.Abs(virtualCamera.m_Lens.OrthographicSize - targetSize) > 0.01f)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                virtualCamera.m_Lens.OrthographicSize,
                targetSize,
                Time.deltaTime * zoomSpeed
            );
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize; // 오차 보정
    }
}
