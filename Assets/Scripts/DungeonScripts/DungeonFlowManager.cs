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
    public DungeonCreator DungeonCreator { get => dungeonCreator; }
    public Vector3 playerSpawnPosition = new Vector3();
    public Vector3 finishSpotPosition = new Vector3();

    // DungeonCreator가 던전 생성 준비를 마쳤으니 던전 생성을 요청할 때 호출하는 Action
    public Action onDungeonCreate;

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
        onDungeonCreate += CreateStage;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    // 스테이지(던전맵, 플레이어 스폰) 생성
    private void CreateStage()
    {
        if (dungeonCreator == null)
        {
            dungeonCreator = FindObjectOfType<DungeonCreator>();
            if (dungeonCreator == null) Debug.LogError("No Dungeon Creator");
        }
        dungeonCreator.CreateFixedRoomDungeon(out playerSpawnPosition);

        // 테스트 플레이어 생성
        GameManager.Instance.PlacePlayerObject(playerSpawnPosition);
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
}

