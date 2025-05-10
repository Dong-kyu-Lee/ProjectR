using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DungeonFlowState
{
    Lobby, Stage1, Stage2, MiddleBoss, Stage3, Stage4, FinalBoss
}

// 던전 스테이지 진행을 관리하는 클래스
public class DungeonFlowManager : MonoBehaviour
{
    private static DungeonFlowManager instance;
    private static DungeonFlowState currentState;
    private GameObject currentFinishSpot;

    [SerializeField]
    private DungeonCreator dungeonCreator;
    public DungeonCreator DungeonCreator 
    { 
        get => dungeonCreator; 
        set { if (dungeonCreator == null) dungeonCreator = value; }
    }

    public GameObject finishSpotPrefab;
    public Vector3 playerSpawnPosition = new Vector3();
    public Vector3 finishSpotPosition = new Vector3();
    public List<RoomInGame> roomList = new List<RoomInGame>();
    private int currentRoomIndex = -1; // 현재 방 인덱스

    public DungeonFlowState GetCurrentDungeonFlow { get => currentState; }
    // DungeonCreator가 던전 생성 준비를 마쳤으니 던전 생성을 요청할 때 호출하는 Action
    public Action onDungeonCreatorReady;

    public static DungeonFlowManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singletonObject = new GameObject("DungeonFlowManager");
                instance = singletonObject.AddComponent<DungeonFlowManager>();
                DontDestroyOnLoad(singletonObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"Duplicate instance of {nameof(DungeonFlowManager)} detected. Destroying the new one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        currentState = DungeonFlowState.Lobby;
        onDungeonCreatorReady += CreateStage;
        DontDestroyOnLoad(gameObject);
    }

    // 스테이지(던전맵, 플레이어 스폰) 생성
    private void CreateStage()
    {
        if (dungeonCreator == null)
        {
            dungeonCreator = FindObjectOfType<DungeonCreator>();
            if (dungeonCreator == null) Debug.LogError("No Dungeon Creator");
        }
        // 던전 생성
        dungeonCreator.CreateFixedRoomDungeon(out playerSpawnPosition, out finishSpotPosition);
        // 테스트 플레이어 생성
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition);
        // 도착 위치 생성
        currentFinishSpot = Instantiate(finishSpotPrefab, finishSpotPosition, transform.rotation);
        Debug.Log("Finish Spot 생성됨. 닫힌 상태");
    }

    private void ResetDungeon()
    {
        if (dungeonCreator != null)
        {
            dungeonCreator.RemoveAllRooms();
        }
        else
        {
            dungeonCreator = FindObjectOfType<DungeonCreator>();
            dungeonCreator.RemoveAllRooms();
        }
    }

    // 현재 스테이지를 기준으로 다음 차례의 스테이지를 정한다.
    public void LoadNextDungeon()
    {
        switch(currentState)
        {
            case DungeonFlowState.Lobby:
                {
                    GameManager.Instance.MoveScene("DungeonGenerate");
                    Debug.Log("Stage1 was Generated");
                    break;
                }
            case DungeonFlowState.Stage1:
                {
                    ResetDungeon();
                    CreateStage();
                    Debug.Log("Stage2 was Generated");
                    break;
                }
            case DungeonFlowState.Stage2:
                {
                    ResetDungeon();
                    // 중간보스 방 프리펩 생성
                    Debug.Log("Middle Boss Room was Generated");
                    break;
                }
            case DungeonFlowState.MiddleBoss:
            case DungeonFlowState.Stage3:
                {
                    ResetDungeon();
                    CreateStage();
                    break;
                }
            case DungeonFlowState.Stage4:
                {
                    // 스테이지 보스 씬으로 이동
                    Debug.Log("Final Boss Room is Generated");
                    break;
                }
            case DungeonFlowState.FinalBoss:
                {
                    // 일반 던전 생성 씬 이동
                    // 최종 스테이지일 경우 엔딩 씬으로 이동
                    break;
                }
        }
        if (currentState != DungeonFlowState.FinalBoss)
            currentState++;
    }

    // 플레이어가 죽었을 때, 던전 진행도를 초기화하기 위한 함수
    public void ResetDungeonFlow()
    {
        currentState = DungeonFlowState.Lobby;
    }

    // roomList에 생성된 방을 추가하는 함수
    public void AddRoomInGame(RoomInGame currentRoom)
    {
        if(currentRoom == null)
        {
            Debug.LogError("추가할 방이 없음");
            return;
        }
        roomList.Add(currentRoom);
    }

    // currentRoom을 클리어하여 다음으로 넘어갈 방의 문을 여는 함수
    public void OpenNextRoom(RoomInGame currentRoom)
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
            roomList.Clear();
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

