using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public enum SceneType
{
    StartScene, LobbyScene, Normal, MiddleBoss, Shop, FinalBossScene, TestScene, StoryScene, EndScene,
}

public class GameManager : MonoBehaviour
{
    #region Member Variables
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

    SoundManager sound = new SoundManager();
    public static SoundManager Sound { get { return instance.sound; } }

    public UnityEvent OnPlayerCharacterChanged = new UnityEvent();

    [SerializeField]
    private GameObject playerObject;
    // 시작 캐릭터 타입(임시).
    // TODO : 추후 기존 저장된 데이터에서 불러오도록 대체할 예정
    public CharacterType startCharacterType;

    private CharacterType currentCharacterType;
    public CharacterType CurrentCharacterType { get => currentCharacterType; }
    public GameObject CurrentPlayer { get => playerObject; }

    [SerializeField]
    private GameObject upgradeUI;
    [SerializeField]
    private GameObject inventoryUI;
    [SerializeField]
    private GameObject inGameUI;
    [SerializeField]
    private GameObject testUI; // 테스트용 UI

    // 임시 변수 : 게임 시작 후 플레이어 최초 생성인지 여부 확인용
    // CharacterSelect에서 게임 시작 후 최초로 Start에서 플레이어 생성
    // 이유 : 원래 StartSceneManager에서 플레이어를 생성했으나, StartScene에서 플레이어를 생성하고 씬 이동을 하면
    // 생성된 플레이어의 PlayerStatus에서 InGameUIManager.Instance를 참조할 때 null 참조 오류가 발생함
    // 래서 LobbyScene에 위치한 CharacterSelect에서 최초 플레이어를 생성하도록 변경함
    public bool isFirstPlayerCreated = false;

    // 캐릭터 프리팹 경로 매핑 - Define 클래스로 이동 예정
    public Dictionary<CharacterType, string> characterPrefabPaths = new Dictionary<CharacterType, string>()
    {
        { CharacterType.Bartender, "Prefabs/Player Prefabs/Bartender2_1 Variant" },
        { CharacterType.Blacksmith, "Prefabs/Player Prefabs/Blacksmith2_1" },
    };

    // 게임 결과 관련 변수
    public TimeSpan totalPlayTimeInSeconds; // 총 플레이 시간(초)
    public int totalKillCount = 0; // 총 처치한 적 수
    public float maximumDamage = 0;
    private DateTime playStartTime;
    private bool isPlayTimeRunning = false;

    // 프롤로그 관리
    public PrologueManager prologue;
    #endregion

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
        instance.sound.Init();
        // 프롤로그 매니저 추가
        prologue = gameObject.AddComponent<PrologueManager>();
    }

    public void SetCurrentPlayer(GameObject value, CharacterType type, Vector3 spawnPosition)
    {
        if(value == null)
        {
            Debug.LogError("SetCurrentPlayer: value is null");
            return;
        }
        Destroy(playerObject); // 이전 플레이어 오브젝트 제거
        playerObject = value;
        currentCharacterType = type;
        playerObject.transform.position = spawnPosition;
        DontDestroyOnLoad(playerObject);
        OnPlayerCharacterChanged?.Invoke();
    }

    // 플레이어 오브젝트를 지정된 시작 지점(position)에 배치하는 함수
    public void PlacePlayerObject(Vector3 position)
    {
        playerObject.transform.position = position;
    }

    /* SceneKey : 이동할 씬의 종류를 나타내는 열거형 , sceneName : 이동할 씬의 이름 */
    public void MoveScene(SceneType key, string sceneName, bool isAsync = false)
    {
        sound.Clear();
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
                SaveManager.Instance.SaveCurrentData();
                // 업그레이드UI & 인벤토리 UI 제거
                DestroyUI();
                // 플레이어 오브젝트 제거
                if (playerObject != null) Destroy(playerObject);
                // 스토리 초기화
                StorySystem.Instance.ResetStory();
                // 게임 결과 초기화
                totalPlayTimeInSeconds = TimeSpan.Zero;
                totalKillCount = 0;
                maximumDamage = 0;
                break;
            case SceneType.Normal:
                // 던전에서 사용되는 UI 생성
                CreateUI();
                SetActiveUI(true);
                break;
            case SceneType.MiddleBoss:
                CreateUI();
                SetActiveUI(true);
                playerObject.SetActive(false);
                break;
            case SceneType.Shop:
                CreateUI();
                SetActiveUI(true);
                break;
            case SceneType.FinalBossScene:
                CreateUI();
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
            case SceneType.EndScene:
                // 업그레이드UI & 인벤토리 UI 비활성화
                SetActiveUI(false);
                // 플레이어 오브젝트 비활성화
                if (playerObject != null) playerObject.SetActive(false);
                // 스토리 초기화
                StorySystem.Instance.ResetStory();
                break;
        }
        if (isAsync)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
        else SceneManager.LoadScene(sceneName);
    }

    // 테스트 씬에서 작동할 던전 씬 UI 생성 함수
    public void InGameUIGenerateForSceneTest()
    {
        // 던전에서 사용되는 UI 생성
        upgradeUI = Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeUICanvas 1.0"));
        inventoryUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/NewQuickSlot"));
        inGameUI = Instantiate(Resources.Load<GameObject>("Prefabs/InGameUICanvas"));
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
        // 스테이지 흐름 초기화
        DungeonFlowManager.Instance.ResetStages();
        // 플레이어를 엔딩 씬으로 이동
        MoveScene(SceneType.EndScene, "EndScene");
        PlayTimeStop();
    }

    public void CreateFirstPlayer()
    {
        playerObject = Instantiate(
            Resources.Load<GameObject>(characterPrefabPaths[startCharacterType]),
            Vector3.zero,
            Quaternion.identity);
        if(playerObject == null)
        {
            Debug.LogError($"CreateFirstPlayer: Player prefab not found at path: {characterPrefabPaths[startCharacterType]}");
            return;
        }
        currentCharacterType = startCharacterType;
        playerObject.SetActive(false);
        DontDestroyOnLoad(playerObject);
    }

    // 플레이 시간 측정 시작/종료 함수
    public void PlayTimeStart()
    {
        if(isPlayTimeRunning) return;
        playStartTime = DateTime.Now;
        isPlayTimeRunning = true;
    }

    public void PlayTimeStop()
    {
        if(!isPlayTimeRunning) return;
        totalPlayTimeInSeconds = DateTime.Now - playStartTime;
        isPlayTimeRunning = false;
    }

    public void SetMaximumDamage(float damage)
    {
        if(damage > maximumDamage)
        {
            maximumDamage = damage;
        }
    }

    // 인게임에 사용되는 UI의 존재를 확인하고 없으면 생성하는 함수
    private void CreateUI()
    {
        if (upgradeUI == null)
        {
            upgradeUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UpgradeUICanvas 1.0"));
            DontDestroyOnLoad(upgradeUI);
        }
        if (inventoryUI == null)
        {
            inventoryUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/NewQuickSlot"));
            DontDestroyOnLoad(inventoryUI);
        }
        if (inGameUI == null)
        {
            inGameUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InGameUICanvas"));
            DontDestroyOnLoad(inGameUI);
        }
        if (testUI != null)
        {
            testUI = Instantiate(DungeonTestHelper.Instance.testUI);
            DontDestroyOnLoad(testUI);
        }
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

    // 인게임 UI 활성화 함수
    // (스토리 씬에서 던전 씬으로 복귀할 때 사용)
    public void SetActiveInGameUI()
    {
        if (inGameUI != null) inGameUI.SetActive(true);
    }
}
