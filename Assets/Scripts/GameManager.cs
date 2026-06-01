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
    // 시작 캐릭터 타입(임시).
    // TODO : 추후 기존 저장된 데이터에서 불러오도록 대체할 예정

    // 현재 플레이어 타입, 오브젝트 리턴(리팩토링 중 의존성 문제로 지울 예정. 잠시 PlayerManager에서 참조)
    public CharacterType CurrentCharacterType { get => PlayerManager.Instance.CurrentCharacterType; }
    public GameObject CurrentPlayer { get => PlayerManager.Instance.CurrentPlayer; }

    [SerializeField]
    private GameObject upgradeUI;
    [SerializeField]
    private GameObject inventoryUI; // [참고] 통합 프리펩 사용 시 더 이상 직접 할당되지 않음 (inGameUI 내부에 포함됨)
    [SerializeField]
    private GameObject inGameUI;

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
        SceneManager.sceneLoaded += FindUpgradeUI;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= FindUpgradeUI;
    }

    /* SceneKey : 이동할 씬의 종류를 나타내는 열거형 , sceneName : 이동할 씬의 이름 */
    public void MoveScene(SceneType key, string sceneName, bool isAsync = false)
    {
        SoundManager.Instance.Clear();
        // 해당 key의 씬으로 이동 시 필요한 코드 실행
        switch (key)
        {
            case SceneType.StartScene:
                // 업그레이드UI & 인벤토리 UI 제거
                DestroyUI();
                // 플레이어 오브젝트 제거
                PlayerManager.Instance.TempDestroyPlayer();
                DungeonFlowManager.Instance.ResetStages();
                break;
            case SceneType.LobbyScene:
                SaveManager.Instance.SaveCurrentData();
                // 업그레이드UI & 인벤토리 UI 제거
                DestroyUI();
                // 플레이어 오브젝트 제거
                PlayerManager.Instance.TempDestroyPlayer();
                // 스토리 초기화
                StorySystem.Instance.ResetStory();
                // 게임 결과 초기화
                GameStatisticsTracker.Instance.ResetStatistics();
                break;
            case SceneType.Normal:
                // 던전에서 사용되는 UI 생성
                CreateUI();
                SetActiveUI(true);
                break;
            case SceneType.MiddleBoss:
                CreateUI();
                SetActiveUI(true);
                PlayerManager.Instance.CurrentPlayer.SetActive(false);
                break;
            case SceneType.Shop:
                CreateUI();
                SetActiveUI(true);
                break;
            case SceneType.FinalBossScene:
                CreateUI();
                SetActiveUI(true);
                PlayerManager.Instance.CurrentPlayer.SetActive(false);
                break;
            case SceneType.TestScene:
                break;
            case SceneType.StoryScene:
                SetActiveUI(false);
                // 플레이어 오브젝트 비활성화
                if (PlayerManager.Instance.CurrentPlayer != null) PlayerManager.Instance.CurrentPlayer.SetActive(false);
                break;
            case SceneType.EndScene:
                // 업그레이드UI & 인벤토리 UI 비활성화
                SetActiveUI(false);
                // 플레이어 오브젝트 비활성화
                if (PlayerManager.Instance.CurrentPlayer != null) PlayerManager.Instance.CurrentPlayer.SetActive(false);
                // 스토리 초기화
                StorySystem.Instance.ResetStory();
                // 업그레이드 능력 초기화
                CalcDamage.Instance.ResetAllEffect();
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
        if (upgradeUI == null) upgradeUI = Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeUICanvas 1.0"));

        // 통합 프리펩 생성
        GameObject integratedObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/GamePlayUI"));
        if (integratedObj != null)
        {
            inGameUI = integratedObj;
            DontDestroyOnLoad(inGameUI);
        }
        else
        {
            Debug.LogError("테스트 씬에서 GamePlayUI를 찾을 수 없음");
        }

        DontDestroyOnLoad(upgradeUI);
    }

    // 인게임에 사용되는 UI의 존재를 확인하고 없으면 생성하는 함수
    private void CreateUI()
    {
        /*if (upgradeUI == null)
        {
            upgradeUI = GameObject.FindObjectOfType<UpgradeUI>()?.gameObject;
            // upgradeUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UpgradeUICanvas 1.0"));
            DontDestroyOnLoad(upgradeUI);
        }*/

        if (inGameUI == null)
        {
            GameObject integratedObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/GamePlayUI"));
            if (integratedObj != null)
            {
                inGameUI = integratedObj;
                DontDestroyOnLoad(inGameUI);
            }
            else
            {
                Debug.LogError("GamePlayUI를 찾을 수 없음");
            }
        }
    }

    // 인게임에 사용되는 UI 제거 함수
    private void DestroyUI()
    {
        if (upgradeUI != null) Destroy(upgradeUI);

        if (inGameUI != null) Destroy(inGameUI);
    }

    // 인게임에 활성화/비활성화 할 UI 설정
    private void SetActiveUI(bool isActive)
    {
        if (upgradeUI != null) upgradeUI.SetActive(isActive);

        if (inGameUI != null) inGameUI.SetActive(isActive);
    }

    // 인게임 UI 활성화 함수
    // (스토리 씬에서 던전 씬으로 복귀할 때 사용)
    public void SetActiveInGameUI()
    {
        if (inGameUI != null) inGameUI.SetActive(true);
    }

    // LobbyScene에 입장했을 때, UpgradeUI를 찾아 할당하는 함수
    private void FindUpgradeUI(Scene scene, LoadSceneMode mode)
    {
        if (upgradeUI == null)
        {
            upgradeUI = GameObject.FindObjectOfType<UpgradeUI>()?.gameObject;
        }
    }
}