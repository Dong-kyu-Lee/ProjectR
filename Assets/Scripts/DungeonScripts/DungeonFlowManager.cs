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
    // 신
    public List<StageData> stageDataList;   // 스테이지 데이터 리스트
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

    // 구
    private static DungeonFlowManager instance;

    public GameObject finishSpotPrefab;

    // DungeonCreator가 던전 생성 준비를 마쳤으니 던전 생성을 요청할 때 호출하는 Action
    // public Action onDungeonCreatorReady;
    

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
        SceneManager.sceneLoaded += SceneChanged;
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

    // 씬이 변경되었을 때, 각 씬에서 DFM이 처리해야 할 작업을 정의하는 함수 (Start 함수와 유사)
    private void SceneChanged(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "DungeonGenerate":
                Debug.Log("DungeonGenerate Scene Loaded");
                if (dungeonCreator == null)
                {
                    dungeonCreator = FindObjectOfType<DungeonCreator>();
                    if (dungeonCreator == null) Debug.LogError("No Dungeon Creator");
                }
                CreateStage();
                break;
        }
    }

    // 스테이지(던전맵, 플레이어 스폰) 생성
    private void CreateStage()
    {
        if (stages.Count > 0)
            return;

        for(int i = 1; i <= stageCount; ++i)
        {
            GameObject stage = new GameObject($"Stage{i}");
            stage.transform.parent = this.transform;
            stage.SetActive(false); // 초기에는 비활성화
            Stage stageComponent = stage.AddComponent<Stage>();
            int stageDataIdx = UnityEngine.Random.Range(0, stageDataList.Count);
            stageComponent.stageData = stageDataList[stageDataIdx];
            stages.Enqueue(stage);
            stageDataList.RemoveAt(stageDataIdx); // 중복 방지
        }

        // 첫 스테이지 오브젝트 활성화
        if (stages.Count > 0) stages.Peek().SetActive(true);
        else Debug.LogError("No stages available to activate");
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
            Debug.LogError("No stages available");
            return null;
        }
        return stages.Peek().GetComponent<Stage>();
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
