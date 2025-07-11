using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageFlow
{
    Lobby, Stage1, Stage2, MiddleBoss, Stage3, Stage4, FinalBoss
}

public class Stage : MonoBehaviour
{
    public StageData stageData;
    private List<RoomInstance> rooms;
    private StageFlow currentState;

    // 일반 던전 관리 변수
    public Vector3 playerSpawnPosition = new Vector3();
    public Vector3 finishSpotPosition = new Vector3();
    private GameObject currentFinishSpot;
    public List<RoomInstance> roomList = new List<RoomInstance>();
    private int currentRoomIndex = -1; // 현재 방 인덱스

    void Start()
    {
        currentState = StageFlow.Lobby;
        Debug.Log($"{gameObject.name} Start");
    }

    void Update()
    {
        
    }

    // 다음 던전을 로드하는 함수
    public void LoadNextDungeon()
    {
        switch (currentState)
        {
            case StageFlow.Lobby:
                // 일반 던전 생성
                break;
            case StageFlow.Stage1:
                break;
            case StageFlow.Stage2:
                break;
            case StageFlow.MiddleBoss:
                break;
            case StageFlow.Stage3:
                break;
            case StageFlow.Stage4:
                break;
            case StageFlow.FinalBoss:
                break;
        }
        if (currentState != StageFlow.FinalBoss)
            currentState++;
    }

    private void CreateDungeon()
    {
        /*if (dungeonCreator == null)
        {
            dungeonCreator = FindObjectOfType<DungeonCreator>();
            if (dungeonCreator == null) Debug.LogError("No Dungeon Creator");
        }
        // 던전 생성
        dungeonCreator.CreateDungeon(out playerSpawnPosition, out finishSpotPosition);
        // 테스트 플레이어 생성
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition);
        // 도착 위치 생성
        currentFinishSpot = Instantiate(finishSpotPrefab, finishSpotPosition, transform.rotation);
        Debug.Log("Finish Spot 생성됨. 닫힌 상태");

        // 던전 갱신 완료 이벤트 호출
        onDungeonReset?.Invoke();*/
    }

    private void ResetDungeon()
    {
        roomList.Clear();
        DungeonFlowManager.Instance.DungeonCreator.RemoveAllRooms();
    }

    // roomList에 생성된 방을 추가하는 함수
    public void AddRoomInstance(RoomInstance currentRoom)
    {
        if (currentRoom == null)
        {
            Debug.LogError("추가할 방이 없음");
            return;
        }
        roomList.Add(currentRoom);
    }

    // currentRoom을 클리어하여 다음으로 넘어갈 방의 문을 여는 함수
    public void OpenNextRoom(RoomInstance currentRoom)
    {
        int index = roomList.IndexOf(currentRoom);
        if (index != -1)
        {
            if (index != roomList.Count - 1) roomList[index + 1].gate.OpenGate(false);
            currentRoomIndex = index;
        }
        else
        {
            Debug.LogError("잘못된 방 데이터 요청");
        }

        // 모든 방을 클리어한 경우
        if (currentRoomIndex == roomList.Count - 1)
        {
            OpenFinishSpot();
        }
    }

    // 스테이지 클리어 시, 다음 스테이지 이동 통로를 활성화하는 함수
    public void OpenFinishSpot()
    {
        Debug.Log("포탈 열림");
        currentFinishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
    }
}
