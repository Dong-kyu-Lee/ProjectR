using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomState { Default, Start, Clear };

// 생성된 방에 대한 조작을 관리하는 클래스
public class RoomInGame : MonoBehaviour
{
    private RoomState roomState = RoomState.Default;
    private bool isFirstWaveEnded = false;

    public RoomState GetRoomState { get => roomState; }
    public Gate gate;
    public GameObject lane;
    public EnemyInRoom enemyInRoom;

    public Action onFirstWaveEnd;
    public Action onSecondWaveEnd;

    void Start()
    {
        onFirstWaveEnd += FirstWaveEnd;
        onSecondWaveEnd += SecondWaveEnd;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && roomState == RoomState.Default)
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
        roomState = RoomState.Clear;
        // 현재 방 문 열기
        gate.OpenGate();
        // 다음 방 문 열기
        DungeonFlowManager.Instance.OpenNextRoom(this);
        lane.SetActive(true);
    }

    public void SetLane(Vector3 generatePosition)
    {

    }

    public void ResetRoomState()
    {
        isFirstWaveEnded = false;
        lane.SetActive(false);
        roomState = RoomState.Default;
    }
}
