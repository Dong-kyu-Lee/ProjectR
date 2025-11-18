using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 생성된 방의 문 조작을 관리하는 클래스
public class Gate : MonoBehaviour
{
    // 위, 오른쪽, 아래, 왼쪽 순서
    public GameObject[] gateObjects;
    public GameObject warpObject;
    public GameObject currentWarp;
    public bool isWarp = false; // 워프가 있는 방인지 확인하는 변수
    public bool[] gateToUse = { false, false, false, false }; // 사용할 방향의 문을 선택

    // 문 오브젝트를 활성화 혹은 비활성화하는 함수
    // 방 생성 시, 필요한 문, 워프 설정
    public void SetUsableDoors(bool[] openNeedGate)
    {
        // 워프 관련 설정 초기화
        isWarp = false;
        if (currentWarp != null) currentWarp.SetActive(false);

        // 위, 아래 문 사용 안 함.
        gateToUse[0] = false;
        gateToUse[2] = false;
        gateObjects[0].SetActive(false);
        gateObjects[2].SetActive(false);

        // 오른쪽, 왼쪽 문 사용 여부 설정
        gateToUse[1] = openNeedGate[1];
        gateObjects[1].SetActive(openNeedGate[1]);
        gateToUse[3] = openNeedGate[3];
        gateObjects[3].SetActive(openNeedGate[3]);
    }

    // warpPosition : 워프가 생성될 월드좌표, playerWarpPosition : 플레이어가 워프될 좌표
    public void CreateWarpObject(Vector3 warpPosition, Vector3 playerWarpPosition)
    {
        isWarp = true; // 워프가 있는 방임을 설정
        if (currentWarp == null)
        {
            currentWarp = Instantiate(warpObject, warpPosition, Quaternion.identity);
        }
        else
        {
            currentWarp.transform.position = warpPosition;
        }
        currentWarp.GetComponent<Warp>().SetWarpPosition(playerWarpPosition);
        currentWarp.SetActive(false);
    }

    // 임시 문 프리팹을 활성화하여 문 열림을 구현
    // 워프가 있는 방일 경우 워프 위치 반환, 없는 방일 경우 Vector3.zero 반환
    public Vector3 OpenGate(bool isRoomCleared)
    {
        for (int i = 0; i < gateObjects.Length; ++i)
        {
            if (gateObjects[i].activeInHierarchy && gateToUse[i])
            {
                gateObjects[i].SetActive(false);
            }
        }
        if (isRoomCleared) // 방을 클리어한 경우: 워프가 같이 열림
        {
            if (isWarp)
            {
                currentWarp.SetActive(true);
            }
        }
        if (isWarp)
        {
            Debug.Log("워프 위치 반환" + currentWarp.transform.position);
            return currentWarp.transform.position;
        }
        return Vector3.zero;
    }

    // 임시 문 프리팹을 비활성화하여 문 열림을 구현
    public void CloseGate()
    {
        for (int i = 0; i < gateObjects.Length; ++i)
        {
            if (!gateObjects[i].activeInHierarchy && gateToUse[i])
            {
                gateObjects[i].SetActive(true);
            }
        }
    }
}
