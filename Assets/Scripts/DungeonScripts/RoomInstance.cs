using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomState { Default, Start, Cleared };

// 생성된 방에 대한 조작을 관리하는 클래스
public class RoomInstance : MonoBehaviour
{
    public RoomState GetRoomState { get => roomState; }
    public Action onFirstWaveEnd;
    public Action onSecondWaveEnd;
    public event Action<RoomState> OnRoomStateChanged; // 외부에서 방 상태 변화를 구독할 수 있는 이벤트
    public Gate GetGate { get => gate; }

    private RoomState roomState = RoomState.Default;
    private bool isFirstWaveEnded = false;
    [SerializeField] private Gate gate;
    [SerializeField] private EnemyInRoom enemyInRoom;
    [SerializeField] private List<GameObject> dynamicElements;
    [SerializeField] private GameObject boxObject;
    private Stage stage; // 이 방이 속한 스테이지
    private GameObject cameraBoundary;

    void Start()
    {
        onFirstWaveEnd += FirstWaveEnd;
        onSecondWaveEnd += SecondWaveEnd;
        dynamicElements = new List<GameObject>();
        cameraBoundary = FindObjectOfType<PolygonCollider2D>().gameObject;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < dynamicElements.Count; ++i)
        {
            Destroy(dynamicElements[i]);
        }
        if (boxObject != null)
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

                // 방 상태가 Start로 변했음을 외부(UI 등)에 알림
                OnRoomStateChanged?.Invoke(roomState);

                // 적 처치 미션 시작
                stage.GetMissionUI.StartMission("모든 적을 처치하세요.", enemyInRoom.killCount, enemyInRoom.totalEnemyCount);
            }
            MoveRoom();
        }
    }

    // 방을 이동할 때 변경되는 요소들 호출 : 카메라 경계, 배경 이미지 이동
    // 플레이어의 방 이동은 워프(Warp.cs)와 직접 이동으로 처리됨.
    private void MoveRoom()
    {
        if(cameraBoundary != null) cameraBoundary.transform.position = transform.position + new Vector3(19.5f, 19.5f, 0);
        else Debug.Log("cameraBoundary is null");
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

        // 방 상태가 Cleared로 변했음을 외부에 알림
        OnRoomStateChanged?.Invoke(roomState);

        // 현재 방 문 열기
        Vector3 arrivePos = gate.OpenGate(true);
        // 다음 방 문 열기
        stage.OpenNextRoom(this, arrivePos);
    }

    public void ResetRoomState()
    {
        isFirstWaveEnded = false;

        roomState = RoomState.Default;

        // 던전 초기화 등으로 인해 방 상태가 Default로 변했음을 알림
        OnRoomStateChanged?.Invoke(roomState);
    }

    public void SetDynamicElements(GameObject dynamicElements)
    {
        for (int i = 0; i < dynamicElements.transform.childCount; ++i)
        {
            GameObject element = Instantiate(dynamicElements.transform.GetChild(i).gameObject,
                transform.position + dynamicElements.transform.GetChild(i).localPosition, Quaternion.identity, transform);
            this.dynamicElements.Add(element);
        }
    }

    // Room 프리팹 내의 상자 오브젝트를 동적으로 생성하는 함수
    public void SetBoxObject(GameObject boxObject)
    {
        this.boxObject = Instantiate(boxObject, transform.position + boxObject.transform.localPosition, Quaternion.identity, transform);
    }

    // Room 프리팹 내의 조명 오브젝트들을 동적으로 생성하는 함수
    public void SetLightObjects(GameObject lights)
    {
        for (int i = 0; i < lights.transform.childCount; ++i)
        {
            GameObject element = Instantiate(lights.transform.GetChild(i).gameObject,
                transform.position + lights.transform.GetChild(i).localPosition, lights.transform.GetChild(i).gameObject.transform.rotation, transform);
            this.dynamicElements.Add(element);
        }
    }

    // RoomInstance가 속한 Stage 참조를 설정하는 함수
    public void SetStageReference(Stage stage)
    {
        this.stage = stage;
    }

    public void SetMissionKillCountText(int killedEnemy, int totalEnemy)
    {
        if (stage.GetMissionUI != null)
        {
            stage.GetMissionUI.SetKillCountText(killedEnemy, totalEnemy);
        }
    }

#if UNITY_EDITOR
    // 에디터용. 방을 즉시 클리어시키는 함수
    public void Editor_EndRoom()
    {
        SecondWaveEnd();
    }
#endif
}