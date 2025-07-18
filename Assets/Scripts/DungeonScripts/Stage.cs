using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum StageFlow
{
    Lobby, Normal1, Normal2, MiddleBoss, Normal3, Shop, FinalBoss
}

public class Stage : MonoBehaviour
{
    public StageData stageData;
    private StageFlow currentArea;

    // 일반 던전 관리 변수
    private int currentRoomIndex = -1; // 현재 방 인덱스
    private GameObject currentFinishSpot; // 현재 일반 던전의 클리어 위치
    public UnityEvent onDungeonReset; // 던전 갱신이 완료되었을 때 호출하는 이벤트
    public UnityEvent onStageClear; // 스테이지 클리어 시 호출하는 이벤트
    public Vector3 playerSpawnPosition = new Vector3();
    public Vector3 finishSpotPosition = new Vector3();
    public List<RoomInstance> roomList = new List<RoomInstance>();

    void Start()
    {
        currentArea = StageFlow.Lobby;
        Debug.Log($"{gameObject.name} Start");
        LoadNextDungeon();
    }

    void Update()
    {
        
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
                GameManager.Instance.MoveScene(SceneKey.MiddleBoss, "TempMiddleBoss");
                break;
            case StageFlow.MiddleBoss:
                GameManager.Instance.MoveScene(SceneKey.Normal, "DungeonGenerate");
                RemoveDungeon();
                CreateDungeon();
                break;
            case StageFlow.Normal3:
                GameManager.Instance.MoveScene(SceneKey.Shop, "ShopScene");
                break;
            case StageFlow.Shop:
                Debug.Log("Final Boss 준비");
                break;
            case StageFlow.FinalBoss:
                // 다음 스테이지로 이동
                break;
        }
        if (currentArea != StageFlow.FinalBoss)
            currentArea++;
    }

    // 던전 씬이 로드될 때까지 기다렸다가 다음 코드를 실행하는 코루틴
    /*IEnumerator CreateDungeonCoroutine()
    {
        yield return new WaitUntil(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="DungeonGenerate")
    }    */

    private void CreateDungeon()
    {
        // 던전 생성
        DungeonFlowManager.Instance.DungeonCreator.CreateDungeon(stageData, out playerSpawnPosition, out finishSpotPosition);
        // 테스트 플레이어 생성
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition);
        // 도착 위치 생성
        currentFinishSpot = Instantiate(DungeonFlowManager.Instance.finishSpotPrefab, finishSpotPosition, transform.rotation);
        Debug.Log("Finish Spot 생성됨. 닫힌 상태");
        // 던전 갱신 완료 이벤트 호출
        onDungeonReset?.Invoke();
    }

    private void RemoveDungeon()
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
