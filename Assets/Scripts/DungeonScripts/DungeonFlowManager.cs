using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// 던전 스테이지 진행을 관리하는 클래스
public class DungeonFlowManager : MonoBehaviour
{
    private bool isSceneChanged = false;
    public List<StageData> stageDataList;   // 스테이지 데이터 리스트
    private List<StageData> stageDataListCopy; // 스테이지 데이터 복사본 (중복 방지용)
    public Queue<GameObject> stages;         // 스테이지 리스트
    public int stageCount = 3;              // 스테이지 개수
    [SerializeField]
    private DungeonCreator dungeonCreator;
    public DungeonCreator DungeonCreator
    {
        get
        {
            if (dungeonCreator == null)
            {
                dungeonCreator = FindObjectOfType<DungeonCreator>();
                if (dungeonCreator == null) Debug.LogError("No Dungeon Creator");
            }
            return dungeonCreator;
        }
    }
    private static DungeonFlowManager instance;
    public GameObject finishSpotPrefab;
    
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
        stages = new Queue<GameObject>();
        SceneManager.sceneLoaded += OnNewStageStarted;
        DontDestroyOnLoad(gameObject);
        if(finishSpotPrefab == null) {
            finishSpotPrefab = Resources.Load<GameObject>("Prefabs/MapElements/FinishSpot");
        }
        if(stageDataList.Count == 0)
        {
            StageData[] stageDataArray = Resources.LoadAll<StageData>("Prefabs/Map Prefabs");
            stageDataList = new List<StageData>(stageDataArray);
        }
    }

    // 새로운 Stage가 선택되고, "DungeonGenerate" 씬이 로드되었을 때 호출되는 함수
    private void OnNewStageStarted(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "DungeonGenerate")
        {
            if (dungeonCreator == null)
            {
                dungeonCreator = FindObjectOfType<DungeonCreator>();
                if (dungeonCreator == null) Debug.LogError("No Dungeon Creator");
            }
            
            if(stages.Count <= 0) CreateStage(); // 첫 스테이지를 시작하는 경우
            else // 다음 스테이지를 실행하는 경우
            {
                if(isSceneChanged)
                {
                    stages.Peek().SetActive(true);
                    isSceneChanged = false;
                }
            }
        }
    }   

    // 스테이지(던전맵, 플레이어 스폰) 생성
    private void CreateStage()
    {
        if (stages.Count > 0)
            return;

        stageDataListCopy = new List<StageData>(stageDataList);
        for (int i = 1; i <= stageCount; ++i)
        {
            GameObject stage = new GameObject($"Stage{i}");
            stage.transform.parent = this.transform;
            stage.SetActive(false); // 초기에는 비활성화
            Stage stageComponent = stage.AddComponent<Stage>();
            int stageDataIdx = UnityEngine.Random.Range(0, stageDataListCopy.Count);
            stageComponent.stageData = stageDataListCopy[stageDataIdx];
            stages.Enqueue(stage);
            stageDataListCopy.RemoveAt(stageDataIdx); // 중복 방지
        }

        // 첫 스테이지 오브젝트 활성화
        if (stages.Count > 0) stages.Peek().SetActive(true);
        else Debug.LogError("No stages available to activate");
    }

    // 스테이지 클리어 후 다음 스테이지를 호출하는 함수
    public void ChangeStage()
    {
        // 완료된 스테이지 제거
        stages.Dequeue().SetActive(false);
        isSceneChanged = true;
        GameManager.Instance.MoveScene(SceneKey.Normal, "DungeonGenerate");
    }

    // 각 스테이지 순서에 따라 정해진 스테이지 데이터를 선택하는 함수
    private void SelectStageData(Stage stage, int stageNum)
    {
        // TODO: 스테이지 데이터 선택 로직 구현
    }

    public Stage GetCurrentStage()
    {
        if (stages.Count == 0)
        {
            Debug.LogWarning("No stages available");
            return null;
        }
        return stages.Peek().GetComponent<Stage>();
    }

    // 플레이어 사망 시, 스테이지 초기화하는 함수
    public void ResetStages()
    {
        while (stages.Count > 0)
        {
            GameObject stage = stages.Dequeue();
            Destroy(stage);
        }
        // 스테이지 리스트 초기화
        stageDataListCopy.Clear();
        stages.Clear();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            SceneManager.sceneLoaded -= (scene, mode) =>
            {
                if (scene.name == "DungeonGenerate")
                {
                    dungeonCreator = null;
                }
            };
        }
    }
}
