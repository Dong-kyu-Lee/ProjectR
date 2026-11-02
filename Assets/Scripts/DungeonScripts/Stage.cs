using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// 스테이지 하나의 진행을 관리하는 클래스
// StageData의 흐름 그래프를 따라 노드(구역)를 이동하며, 노드 타입에 맞는 상태(IStageState)가 구역 진입/이탈을 처리한다.
public class Stage : MonoBehaviour
{
    public StageData stageData;

    // 스테이지 흐름 상태 머신
    private StageGraph graph;
    private StageNode currentNode;
    private IStageState currentState;
    public StageNode CurrentNode { get => currentNode; }
    // 현재 노드의 목표(ex. 보스 처치)를 달성했는지 여부. 컷씬에서 복귀해 씬이 다시 로드되어도 유지된다.
    public bool IsCurrentNodeCleared { get; set; }

    // 씬 로드 후 실행할 작업
    private string pendingSceneName;
    private Action pendingOnSceneLoaded;

    // 일반 던전 관리 변수
    private int currentRoomIndex = -1; // 현재 방 인덱스
    private GameObject currentFinishSpot; // 현재 일반 던전의 클리어 위치
    // Stage는 AddComponent로 생성되어 직렬화 초기화가 되지 않으므로 직접 초기화한다.
    public UnityEvent onDungeonReset = new UnityEvent(); // 던전 갱신이 완료되었을 때 호출하는 이벤트
    public UnityEvent onStageClear = new UnityEvent(); // 스테이지 클리어 시 호출하는 이벤트
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
        Debug.Log($"{gameObject.name} Start");
        graph = new StageGraph(stageData);
        if (graph.StartNode == null)
        {
            Debug.LogError($"{gameObject.name}: 시작 노드가 없어 스테이지를 진행할 수 없음");
            return;
        }
        EnterNode(graph.StartNode);
        GameStatisticsTracker.Instance.PlayTimeStart();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnPendingSceneLoaded;
    }

    // 현재 노드를 완료하고 그래프의 다음 노드로 이동하는 함수
    // 다음 노드가 없으면 이 스테이지를 클리어한 것으로 보고 다음 스테이지로 이동한다.
    // choiceIndex : PlayerChoice 분기에서 플레이어가 고른 간선 인덱스
    public void CompleteCurrentNode(int choiceIndex = -1)
    {
        if (currentNode == null)
        {
            Debug.LogError($"{gameObject.name}: 진행 중인 노드가 없음");
            return;
        }

        StageNode nextNode = graph.SelectNext(currentNode, choiceIndex);
        currentState?.Exit(this);

        if (nextNode == null)
        {
            // 마지막 스테이지였는지 판단과 EndScene 이동은 DungeonFlowManager가 처리한다.
            currentNode = null;
            currentState = null;
            onStageClear?.Invoke();
            DungeonFlowManager.Instance.ChangeStage();
            return;
        }
        EnterNode(nextNode);
    }

    private void EnterNode(StageNode node)
    {
        Debug.Log($"{gameObject.name}: '{node.id}' ({node.type}) 노드 진입");
        currentNode = node;
        IsCurrentNodeCleared = false;
        currentState = StageStateFactory.Create(node.type);
        currentState.Enter(this, node);
    }

    // 씬을 이동하고, 해당 씬의 로드가 끝나면 onLoaded를 실행하는 함수
    public void LoadSceneThen(Define.SceneType sceneType, string sceneName, Action onLoaded, bool isAsync = false)
    {
        SceneManager.sceneLoaded -= OnPendingSceneLoaded;
        pendingSceneName = sceneName;
        pendingOnSceneLoaded = onLoaded;
        if (onLoaded != null)
            SceneManager.sceneLoaded += OnPendingSceneLoaded;

        GameManager.Instance.MoveScene(sceneType, sceneName, isAsync);
    }

    private void OnPendingSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != pendingSceneName) return;

        SceneManager.sceneLoaded -= OnPendingSceneLoaded;
        Action onLoaded = pendingOnSceneLoaded;
        pendingSceneName = null;
        pendingOnSceneLoaded = null;
        onLoaded?.Invoke();
    }

    public void CreateDungeon(StageNode node)
    {
        // 던전 생성 전, 플레이어 추락 방지를 위해 플레이어 비활성화
        PlayerManager.Instance.CurrentPlayer.SetActive(false);
        // 던전 생성
        DungeonCreator dungeonCreator = FindObjectOfType<DungeonCreator>();
        if (dungeonCreator == null)
        {
            Debug.LogError("DungeonCreator를 찾을 수 없음");
            return;
        }
        dungeonCreator.CreateDungeon(stageData, node.GetNumberOfRooms(stageData), node.GetDungeonBoxCount(stageData),
            out playerSpawnPosition, out finishSpotPosition);
        // 테스트 플레이어 생성
        PlayerManager.Instance.PlacePlayerObject(playerSpawnPosition);
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
        StartCoroutine(PlayerActivateDelay());
    }

    public void RemoveDungeon()
    {
        roomList.Clear();
        currentRoomIndex = -1;
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
                roomList[index + 1].GetGate.OpenGate(false);
                if (arrivePos != Vector3.zero) // 워프를 통한 이동인 경우
                    missionUI.StartMission("다음 방으로 이동하세요.", arrivePos);
                else // 통로를 통한 이동인 경우, 다음 방의 중심 좌표를 목표 위치로 설정
                    missionUI.StartMission("다음 방으로 이동하세요.", roomList[index + 1].transform.position + new Vector3(20, 20, 0));
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
            missionUI.StartMission("다음 방으로 이동하세요.", currentFinishSpot.transform.position);
        }
    }

    // 스테이지 클리어 시, 다음 스테이지 이동 통로를 활성화하는 함수
    public void OpenFinishSpot()
    {
        Debug.Log("포탈 열림");
        IsCurrentNodeCleared = true;
        currentFinishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
    }

    private IEnumerator PlayerActivateDelay()
    {
        yield return new WaitForSeconds(1f);
        PlayerManager.Instance.CurrentPlayer.SetActive(true);
    }
}
