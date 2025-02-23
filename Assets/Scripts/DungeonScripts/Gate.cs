using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 생성된 방의 문 조작을 관리하는 클래스
public class Gate : MonoBehaviour
{
    // 위, 오른쪽, 아래, 왼쪽 순서
    public GameObject[] gateObjects;
    public bool[] gateToUse = { false, false, false, false }; // 사용할 방향의 문을 선택

    // 문 오브젝트를 활성화 혹은 비활성화하는 함수
    public void SetUsableDoors(bool[] openNeedGate)
    {
        for(int i = 0; i < openNeedGate.Length; ++i)
        {
            if (openNeedGate[i] == true)
            {
                gateToUse[i] = true;
                gateObjects[i].SetActive(true);
            }
            else
            {
                gateToUse[i] = false;
                gateObjects[i].SetActive(false);
            }
        }
    }

    // 임시 문 프리팹을 활성화하여 문 열림을 구현
    public void OpenGate()
    {
        for(int i = 0; i < gateObjects.Length; ++i)
        {
            if(gateObjects[i].activeInHierarchy && gateToUse[i])
            {
                gateObjects[i].SetActive(false);
            }
        }
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
