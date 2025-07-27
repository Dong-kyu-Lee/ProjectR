using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomState { Default, Start, Cleared };

// 생성된 방에 대한 조작을 관리하는 클래스
public class RoomInstance : MonoBehaviour
{
    private RoomState roomState = RoomState.Default;
    private bool isFirstWaveEnded = false;

    public RoomState GetRoomState { get => roomState; }
    public Gate gate;
    public EnemyInRoom enemyInRoom;
    public List<GameObject> dynamicElements;
    public GameObject boxObject;

    public Action onFirstWaveEnd;
    public Action onSecondWaveEnd;

    void Start()
    {
        onFirstWaveEnd += FirstWaveEnd;
        onSecondWaveEnd += SecondWaveEnd;
        dynamicElements = new List<GameObject>();
    }

    private void OnDestroy()
    {
        for(int i = 0; i < dynamicElements.Count; ++i)
        {
            Destroy(dynamicElements[i]);
        }
        if(boxObject != null)
        {
            Destroy(boxObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (roomState == RoomState.Default)
            {
                // 문 닫기
                gate.CloseGate();

                Debug.Log("Player is Detected");
                if (isFirstWaveEnded == false)
                {
                    // first wave 적 생성
                    enemyInRoom.ActivateEnemies(false);
                }
                // 방 상태 변경
                roomState = RoomState.Start;
            }
            // 카메라 경계 현재 방 위치로 이동
            DungeonFlowManager.Instance.DungeonCreator.cameraBoundary.transform.position = transform.position + new Vector3(19.5f, 19.5f, 0);
        }
    }

    // 첫번재 웨이브의 적들을 모두 처치했을 때 실행되는 함수
    private void FirstWaveEnd()
    {
        isFirstWaveEnded = true;
        // second wave 적 생성
        enemyInRoom.ActivateEnemies(true);
    }

    // 두번째 웨이브의 적을을 모두 처치했을 때 실행되는 함수
    private void SecondWaveEnd()
    {
        // 방 상태 변경
        roomState = RoomState.Cleared;
        // 현재 방 문 열기
        gate.OpenGate(true);
        // 다음 방 문 열기
        DungeonFlowManager.Instance.GetCurrentStage().OpenNextRoom(this);
    }

    public void ResetRoomState()
    {
        isFirstWaveEnded = false;
        roomState = RoomState.Default;
    }

    public void SetDynamicElements(GameObject dynamicElements)
    {
        for(int i = 0; i < dynamicElements.transform.childCount; ++i)
        {
            GameObject element = Instantiate(dynamicElements.transform.GetChild(i).gameObject, 
                transform.position + dynamicElements.transform.GetChild(i).localPosition, Quaternion.identity, transform);
            this.dynamicElements.Add(element);
        }
    }

    public void SetBoxObject(GameObject boxObject)
    {
        this.boxObject = Instantiate(boxObject, transform.position + boxObject.transform.localPosition, Quaternion.identity, transform);
    }
}
