using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 던전 씬에 사용되는 시네머신을 관리하는 스크립트
public class CM_DungeonScene : MonoBehaviour
{
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        virtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = GameManager.Instance.CurrentPlayer.transform;
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera not found on this GameObject.");
        }
    }
}
