using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum StageFlow
{
    Lobby, Normal1, Normal2, MiddleBoss, Normal3, Shop, FinalBoss
}

public class Stage : MonoBehaviour
{
    public StageData stageData;
    private StageFlow currentArea = StageFlow.Lobby;
    public StageFlow GetCurrentArea { get => currentArea; }

    // 일반 던전 관리 변수
    private int currentRoomIndex = -1; // 현재 방 인덱스
    private GameObject currentFinishSpot; // 현재 일반 던전의 클리어 위치
    public UnityEvent onDungeonReset; // 던전 갱신이 완료되었을 때 호출하는 이벤트
    public UnityEvent onStageClear; // 스테이지 클리어 시 호출하는 이벤트
    public Vector3 playerSpawnPosition = new Vector3();
    public Vector3 finishSpotPosition = new Vector3();
    public List<RoomInstance> roomList = new List<RoomInstance>();

    private MissionUI missionUI;
    public MissionUI GetMissionUI
    {
        get
        {
            if(missionUI == null)
            {
                missionUI = FindObjectOfType<MissionUI>();
            }
            return missionUI;
        }
    }

    void Start()
    {
        currentArea = StageFlow.Lobby;
        Debug.Log($"{gameObject.name} Start");
        LoadNextDungeon();
    }

    // 다음 던전 구역을 로드하는 함수
    public void LoadNextDungeon()
    {
        switch (currentArea)
        {
            case StageFlow.Lobby:
                // 일반 던전 생성
                CreateDungeon();
                break;
            case StageFlow.Normal1:
                RemoveDungeon();
                CreateDungeon();
                break;
            case StageFlow.Normal2:
                GameManager.Instance.MoveScene(SceneType.MiddleBoss, "TempMiddleBoss");
                break;
            case StageFlow.MiddleBoss:
                RemoveDungeon();
                MoveToDungeonAndCreate();
                break;
            case StageFlow.Normal3:
                GameManager.Instance.MoveScene(SceneType.Shop, "ShopScene");
                break;
            case StageFlow.Shop:
                GameManager.Instance.MoveScene(SceneType.FinalBossScene, "TempFinalBoss");
                break;
            case StageFlow.FinalBoss:
                // Demo 버전 - End Scene으로 이동
                GameManager.Instance.MoveScene(SceneType.EndScene, "EndScene");
                // 다음 스테이지로 이동
                // DungeonFlowManager.Instance.ChangeStage();
                break;
        }
        if (currentArea != StageFlow.FinalBoss)
            currentArea++;
    }

    private void CreateDungeon()
    {
        // 던전 생성
        DungeonFlowManager.Instance.DungeonCreator.CreateDungeon(stageData, out playerSpawnPosition, out finishSpotPosition);
        // 테스트 플레이어 생성
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition);
        // 도착 위치 생성
        currentFinishSpot = Instantiate(DungeonFlowManager.Instance.finishSpotPrefab, finishSpotPosition, transform.rotation);
        Debug.Log("Finish Spot 생성됨. 닫힌 상태");
        // MissionUI 참조 가져오기
        missionUI = FindObjectOfType<MissionUI>();
        if(missionUI == null)
        {
            Debug.LogError("MissionUI를 찾을 수 없음");
        }
        // 던전 갱신 완료 이벤트 호출
        onDungeonReset?.Invoke();
    }

    private void RemoveDungeon()
    {
        roomList.Clear();
    }

    // roomList에 생성된 방을 추가하는 함수.
    // 던전 생성 시 DungeonCreator에서 생성한 RoomInstance를 Stage에 추가하기 위해 사용
    public void AddRoomInstance(RoomInstance currentRoom)
    {
        if (currentRoom == null)
        {
            Debug.LogError("추가할 방이 없음");
            return;
        }
        roomList.Add(currentRoom);
        currentRoom.SetStageReference(this);
    }

    // currentRoom을 클리어하여 다음으로 넘어갈 방의 문을 여는 함수
    // arrivePos : 미션에서 사용. 플레이어가 도착해야 할 위치
    // arrivePos == Vector3.zero : 통로를 통한 이동. 특정 위치를 나타내지 않음.
    // arrivePos != Vector3.zero : 워프를 통한 이동. 워프의 위치를 나타냄.
    public void OpenNextRoom(RoomInstance currentRoom, Vector3 arrivePos)
    {
        int index = roomList.IndexOf(currentRoom);
        if (index != -1)
        {
            if (index != roomList.Count - 1)
            {
                roomList[index + 1].gate.OpenGate(false);
                if (arrivePos != Vector3.zero) // 워프를 통한 이동인 경우
                    missionUI.StartMission("Move to next room.", arrivePos);
                else // 통로를 통한 이동인 경우, 다음 방의 중심 좌표를 목표 위치로 설정
                    missionUI.StartMission("Move to next room.", roomList[index + 1].transform.position + new Vector3(20, 20, 0));
            }
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
            missionUI.StartMission("Move to next room.", currentFinishSpot.transform.position);
        }
    }

    // 스테이지 클리어 시, 다음 스테이지 이동 통로를 활성화하는 함수
    public void OpenFinishSpot()
    {
        Debug.Log("포탈 열림");
        currentFinishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
    }


    private void MoveToDungeonAndCreate()
    {
        SceneManager.sceneLoaded += OnDungeonSceneLoaded;
        GameManager.Instance.MoveScene(SceneType.Normal, "DungeonGenerate", true);
    }

    private void OnDungeonSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "DungeonGenerate")
        {
            CreateDungeon();
        }
        SceneManager.sceneLoaded -= OnDungeonSceneLoaded;
    }
}
