using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 로비 씬에 사용되는 시네머신을 관리하는 스크립트
public class CM_LobbyScene : MonoBehaviour
{
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;

    void Start()
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
}
