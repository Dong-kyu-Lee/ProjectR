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

    [SerializeField]
    private DungeonCreator dungeonCreator;
    public DungeonCreator DungeonCreator 
    { 
        get => dungeonCreator; 
        set { if (dungeonCreator == null) dungeonCreator = value; }
    }
    [SerializeField]
    private EnemySpawnManager enemySpawnManager;
    public EnemySpawnManager EnemySpawnManager
    {
        get => enemySpawnManager;
        set { if (enemySpawnManager == null) enemySpawnManager = value; }
    }

    public GameObject finishSpotPrefab;
    private GameObject currentFinishSpot;
    public Vector3 playerSpawnPosition = new Vector3();
    public Vector3 finishSpotPosition = new Vector3();

    // DungeonCreator가 던전 생성 준비를 마쳤으니 던전 생성을 요청할 때 호출하는 Action
    public Action onDungeonCreatorReady;

    public DungeonFlowState GetCurrentDungeonFlow { get => currentState; }

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
        // dungeonCreator.CreateRandomRoomDungeon();
        // 테스트 플레이어 생성
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition);
        // 도착 위치 생성
        currentFinishSpot = Instantiate(finishSpotPrefab, finishSpotPosition, transform.rotation);
        Debug.Log("Finish Spot 생성됨. 닫힌 상태");
        // 적 생성
        if (GameManager.Instance.isCreateEnemies == true)
            dungeonCreator.gameObject.GetComponent<EnemySpawnManager>().GenerateEnemies();
        else
            OpenFinishSpot();
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
                    SceneManager.LoadScene("DungeonGenerate");
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

    public void ResetDungeonFlow()
    {
        currentState = DungeonFlowState.Lobby;
    }

    public void OpenFinishSpot()
    {
        currentFinishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
    }
}

