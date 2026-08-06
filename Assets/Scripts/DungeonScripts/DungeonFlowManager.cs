using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 던전 스테이지 진행을 관리하는 클래스
// 한 판은 stagePools의 개수만큼 스테이지로 구성되며,
// 각 스테이지는 해당 순서의 StagePool에서 StageData 하나를 랜덤 선택해 진행한다.
public class DungeonFlowManager : MonoBehaviour
{
    [Header("Stage Flow")]
    [Tooltip("스테이지 순서대로의 후보 풀. 리스트 크기가 한 판의 스테이지 수가 된다. (기획상 4개)")]
    public List<StagePool> stagePools = new List<StagePool>();

    [Tooltip("한 판 안에서 같은 StageData가 두 번 선택되지 않도록 함")]
    public bool avoidDuplicateStageData = true;

    [Header("Needed Objects")]
    public GameObject finishSpotPrefab;
    [SerializeField] private DungeonCreator dungeonCreator;

    private readonly List<GameObject> stages = new List<GameObject>();           // 이번 판의 스테이지 오브젝트
    private readonly List<StageData> selectedStageDatas = new List<StageData>(); // 이번 판에 확정된 StageData
    private int currentStageIndex = -1;
    private bool isSceneChanged = false;

    public int StageCount { get => stages.Count; }
    public int CurrentStageNumber { get => currentStageIndex + 1; }   // UI 표시용 (1-based)
    public bool IsLastStage { get => currentStageIndex >= stages.Count - 1; }
    public IReadOnlyList<StageData> SelectedStageDatas { get => selectedStageDatas; }

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
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnNewStageStarted;

        if (finishSpotPrefab == null)
            finishSpotPrefab = Resources.Load<GameObject>("Prefabs/MapElements/FinishSpot");

        if (stagePools == null || stagePools.Count == 0)
        {
            // 인스펙터 설정이 없는 씬에서도 동작하도록 하는 임시 대비책
            BuildFallbackPools();
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            SceneManager.sceneLoaded -= OnNewStageStarted;
        }
    }

    // 새로운 Stage가 선택되고, "DungeonGenerate" 씬이 로드되었을 때 호출되는 함수
    private void OnNewStageStarted(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "DungeonGenerate") return;

        dungeonCreator = FindObjectOfType<DungeonCreator>();
        if (dungeonCreator == null) Debug.LogError("No Dungeon Creator");

        if (stages.Count == 0)
        {
            CreateStages();   // 첫 스테이지를 시작하는 경우
        }
        else if (isSceneChanged)
        {
            ActivateCurrentStage();   // 다음 스테이지를 실행하는 경우
            isSceneChanged = false;
        }
    }

    // 한 판에 사용할 스테이지를 슬롯 순서대로 확정하고 스테이지 오브젝트를 생성하는 함수
    private void CreateStages()
    {
        if (stages.Count > 0) return;

        selectedStageDatas.Clear();

        for (int i = 0; i < stagePools.Count; ++i)
        {
            StagePool pool = stagePools[i];
            if (pool == null || pool.IsValid == false)
            {
                Debug.LogError($"{i + 1}번째 스테이지 풀이 비어 있어 해당 스테이지를 건너뜀");
                continue;
            }

            StageData stageData = pool.PickRandom(avoidDuplicateStageData ? selectedStageDatas : null);
            if (stageData == null) continue;

            selectedStageDatas.Add(stageData);
            stages.Add(CreateStageObject(stages.Count + 1, stageData));
        }

        if (stages.Count == 0)
        {
            Debug.LogError("생성된 스테이지가 없음. StagePool 설정을 확인할 것");
            return;
        }

        currentStageIndex = 0;
        ActivateCurrentStage();
    }

    // 스테이지 오브젝트 하나를 생성하는 함수
    // 비활성 상태로 만들어 두고, 활성화되는 시점에 Stage.Start()에서 던전이 생성된다.
    private GameObject CreateStageObject(int stageNumber, StageData stageData)
    {
        GameObject stageObject = new GameObject($"Stage{stageNumber}_{stageData.name}");
        stageObject.transform.parent = this.transform;
        stageObject.SetActive(false);

        Stage stage = stageObject.AddComponent<Stage>();
        stage.stageData = stageData;
        return stageObject;
    }

    private void ActivateCurrentStage()
    {
        if (currentStageIndex < 0 || currentStageIndex >= stages.Count)
        {
            Debug.LogError($"잘못된 스테이지 인덱스: {currentStageIndex}");
            return;
        }
        stages[currentStageIndex].SetActive(true);
    }

    // 스테이지 클리어 후 다음 스테이지를 호출하는 함수
    // 마지막 스테이지였다면 한 판이 끝난 것으로 처리한다.
    public void ChangeStage()
    {
        if (currentStageIndex < 0 || stages.Count == 0)
        {
            Debug.LogError("진행 중인 스테이지가 없음");
            return;
        }

        stages[currentStageIndex].SetActive(false);

        if (IsLastStage)
        {
            OnAllStagesCleared();
            return;
        }

        currentStageIndex++;
        isSceneChanged = true;
        GameManager.Instance.MoveScene(SceneType.Normal, "DungeonGenerate");
    }

    // 한 판의 모든 스테이지를 클리어했을 때 호출되는 함수
    private void OnAllStagesCleared()
    {
        GameStatisticsTracker.Instance.PlayTimeStop();
        GameManager.Instance.MoveScene(SceneType.EndScene, "EndScene");
    }

    public Stage GetCurrentStage()
    {
        if (currentStageIndex < 0 || currentStageIndex >= stages.Count)
        {
            Debug.LogWarning("No stages available");
            return null;
        }
        return stages[currentStageIndex].GetComponent<Stage>();
    }

    // 플레이어 사망 시, 스테이지 초기화하는 함수
    public void ResetStages()
    {
        for (int i = 0; i < stages.Count; ++i)
        {
            if (stages[i] != null) Destroy(stages[i]);
        }
        stages.Clear();
        selectedStageDatas.Clear();
        currentStageIndex = -1;
        isSceneChanged = false;
    }

    // StagePool이 설정되지 않은 경우, Resources의 StageData 전체를 후보로 하는 풀을 임시 생성
    private void BuildFallbackPools()
    {
        const int fallbackStageCount = 4;

        StageData[] loaded = Resources.LoadAll<StageData>("Prefabs/Map Prefabs/StageAssets");
        if (loaded.Length == 0)
        {
            Debug.LogError("StagePool이 비어 있고 StageData도 찾을 수 없음");
            return;
        }

        Debug.LogWarning("StagePool이 설정되지 않아 임시 풀을 생성함. 인스펙터에서 StagePool을 지정할 것");

        stagePools = new List<StagePool>();
        for (int i = 0; i < fallbackStageCount; ++i)
        {
            StagePool pool = ScriptableObject.CreateInstance<StagePool>();
            pool.name = $"FallbackPool{i + 1}";
            pool.candidates = new List<StageData>(loaded);
            stagePools.Add(pool);
        }
    }
}
