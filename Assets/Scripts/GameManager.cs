using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum SceneType
{
    StartScene, LobbyScene, Normal, MiddleBoss, Shop, FinalBossScene, TestScene, StoryScene, TempEndScene,
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public UnityEvent OnPlayerCharacterChanged = new UnityEvent();

    [SerializeField]
    private GameObject playerObject;
    public GameObject CurrentPlayer { 
        get => playerObject;
        set
        {
            playerObject = value;
            DontDestroyOnLoad(playerObject);
            OnPlayerCharacterChanged?.Invoke();
        }
    }

    [SerializeField]
    private GameObject upgradeUI;
    [SerializeField]
    private GameObject inventoryUI;
    [SerializeField]
    private GameObject inGameUI;
    [SerializeField]
    private GameObject testUI; // 테스트용 UI


    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복된 GameManager 제거
        }
    }

    // 플레이어 오브젝트를 지정된 시작 지점(position)에 배치하는 함수
    public void PlacePlayerObject(Vector3 position)
    {
        playerObject.transform.position = position;
    }

    /* SceneKey : 이동할 씬의 종류를 나타내는 열거형 , sceneName : 이동할 씬의 이름 */
    public void MoveScene(SceneType key, string sceneName)
    {
        // 해당 key의 씬으로 이동 시 필요한 코드 실행
        switch(key)
        {
            case SceneType.StartScene:
                // 업그레이드UI & 인벤토리 UI 제거
                DestroyUI();
                // 플레이어 오브젝트 제거
                Destroy(playerObject);
                DungeonFlowManager.Instance.ResetStages();
                break;
            case SceneType.LobbyScene:
                // 업그레이드UI & 인벤토리 UI 제거
                DestroyUI();
                // 플레이어 오브젝트 제거
                if (playerObject != null) Destroy(playerObject);
                // 스토리 초기화
                StorySystem.Instance.ResetStory();
                break;
            case SceneType.Normal:
                // 던전에서 사용되는 UI 생성
                if (upgradeUI == null)
                {
                    upgradeUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UpgradeUICanvas 1.0"));
                    DontDestroyOnLoad(upgradeUI);
                }
                if (inventoryUI == null)
                {
                    inventoryUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas(QuickSlot)"));
                    DontDestroyOnLoad(inventoryUI);
                }
                if (inGameUI == null)
                {
                    inGameUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InGameUICanvasV2"));
                    DontDestroyOnLoad(inGameUI);
                }
                if (testUI != null)
                {
                    testUI = Instantiate(DungeonTestHelper.Instance.testUI);
                    DontDestroyOnLoad(testUI);
                }
                SetActiveUI(true);
                break;
            case SceneType.MiddleBoss:
                SetActiveUI(true);
                playerObject.SetActive(false);
                break;
            case SceneType.Shop:
                break;
            case SceneType.FinalBossScene:
                SetActiveUI(true);
                playerObject.SetActive(false);
                break;
            case SceneType.TestScene:
                break;
            case SceneType.StoryScene:
                SetActiveUI(false);
                // 플레이어 오브젝트 비활성화
                if (playerObject != null) playerObject.SetActive(false);
                break;
            case SceneType.TempEndScene: // 동아리의 밤 행사까지만 쓸 엔딩 씬
                // 업그레이드UI & 인벤토리 UI 제거
                DestroyUI();
                // 플레이어 오브젝트 비활성화
                if (playerObject != null) playerObject.SetActive(false);
                // 스토리 초기화
                StorySystem.Instance.ResetStory();
                break;
        }
        SceneManager.LoadScene(sceneName);
    }

    // 테스트 씬에서 작동할 던전 씬 UI 생성 함수
    public void InGameUIGenerateForSceneTest()
    {
        // 던전에서 사용되는 UI 생성
        upgradeUI = Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeUICanvas 1.0"));
        inventoryUI = Instantiate(Resources.Load<GameObject>("Prefabs/Canvas(QuickSlot)"));
        inGameUI = Instantiate(Resources.Load<GameObject>("Prefabs/InGameUICanvasV2"));
        DontDestroyOnLoad(upgradeUI);
        DontDestroyOnLoad(inventoryUI);
        DontDestroyOnLoad(inGameUI);

        if (testUI != null)
        {
            testUI = Instantiate(DungeonTestHelper.Instance.testUI);
            DontDestroyOnLoad(testUI);
        }
    }

    // 플레이어 사망 시, PlayerControllerBase에서 호출되는 함수
    public void PlayerDead()
    {
        // 게임 오버 UI 표시

        // 스테이지 흐름 초기화
        DungeonFlowManager.Instance.ResetStages();
        // 플레이어를 로비 씬으로 이동
        MoveScene(SceneType.LobbyScene, "LobbyScene");
    }

    // 인게임에 사용되는 UI 제거 함수
    private void DestroyUI()
    {
        if (upgradeUI != null) Destroy(upgradeUI);
        if (inventoryUI != null) Destroy(inventoryUI);
        if (inGameUI != null) Destroy(inGameUI);
        if (testUI != null) Destroy(testUI);
    }

    // 인게임에 활성화/비활성화 할 UI 설정
    private void SetActiveUI(bool isActive)
    {
        if (upgradeUI != null) upgradeUI.SetActive(isActive);
        if (inventoryUI != null) inventoryUI.SetActive(isActive);
        if (inGameUI != null) inGameUI.SetActive(isActive);
        if (testUI != null) testUI.SetActive(isActive);
    }
}
