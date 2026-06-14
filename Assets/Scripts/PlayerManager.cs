using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플레이어 오브젝트의 생성, 교체를 담당하는 매니저 클래스
/// GameManager 오브젝트에 부착된 싱글톤 클래스로, GameManager와 같은 생명주기를 가짐.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerManager>();
                if (instance == null)
                {
                    Debug.LogWarning("PlayerManager 가 씬에 존재하지 않습니다. GameManager 오브젝트에 PlayerManager 컴포넌트를 추가합니다.");
                    instance = GameManager.Instance.gameObject.AddComponent<PlayerManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private GameObject playerObject;
    public GameObject CurrentPlayer { get => playerObject; }

    [SerializeField]
    private CharacterDatabase characterDatabase;
    private CharacterType currentCharacterType;
    private CharacterType startCharacterType;
    public CharacterType CurrentCharacterType { get => currentCharacterType; }

    // CharacterSelect에서 게임 시작 후 최초로 Start에서 플레이어 생성
    // 이유 : 원래 StartSceneManager에서 플레이어를 생성했으나, StartScene에서 플레이어를 생성하고 씬 이동을 하면
    // 생성된 플레이어의 PlayerStatus에서 InGameUIManager.Instance를 참조할 때 null 참조 오류가 발생함
    // 그래서 LobbyScene에 위치한 CharacterSelect에서 최초 플레이어를 생성하도록 변경함
    private bool isFirstPlayerCreated = false;
    public bool IsFirstPlayerCreated { get => isFirstPlayerCreated; }

    // 플레이어 캐릭터 변경 시 이벤트
    public UnityEvent OnPlayerCharacterChanged = new UnityEvent();

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject.GetComponent<PlayerManager>());
        }
        GameManager.Instance.OnSceneChanged -= OnSceneChanged; // 중복 구독 방지
        GameManager.Instance.OnSceneChanged += OnSceneChanged;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnSceneChanged -= OnSceneChanged;
    }

    public GameObject GetCharacterPrefab(CharacterType type)
    {
        CharacterData data = characterDatabase.characterDataList[(int)type];
        return data.characterPrefab;
    }

    public void SetCurrentPlayer(GameObject value, CharacterType type, Vector3 spawnPosition)
    {
        if (value == null)
        {
            Debug.LogError("SetCurrentPlayer: value is null");
            return;
        }
        Destroy(playerObject); // 이전 플레이어 오브젝트 제거
        playerObject = value;
        currentCharacterType = type;
        playerObject.transform.position = spawnPosition;
        playerObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2; // 로비 씬에서 다른 캐릭터와 겹치지 않기 위함
        DontDestroyOnLoad(playerObject);
        OnPlayerCharacterChanged?.Invoke();
    }

    // 플레이어 오브젝트를 지정된 시작 지점(position)에 배치하는 함수
    public void PlacePlayerObject(Vector3 position)
    {
        playerObject.transform.position = position;
    }

    // 플레이어 사망 시, PlayerControllerBase에서 호출되는 함수
    public void PlayerDead()
    {
        // 스테이지 흐름 초기화
        DungeonFlowManager.Instance.ResetStages();
        // 플레이어를 엔딩 씬으로 이동
        GameManager.Instance.MoveScene(SceneType.EndScene, "EndScene");
        GameStatisticsTracker.Instance.PlayTimeStop();
    }

    // 임시 함수 : 플레이어 오브젝트 제거
    public void TempDestroyPlayer()
    {
        if(playerObject != null)
        {
            Destroy(playerObject);
            playerObject = null;
        }
    }

    public void CreateFirstPlayer()
    {
        playerObject = Instantiate(
            characterDatabase.characterDataList[(int)startCharacterType].characterPrefab,
            Vector3.zero,
            Quaternion.identity
        );
        if (playerObject == null)
        {
            return;
        }
        currentCharacterType = startCharacterType;
        playerObject.SetActive(false);
        playerObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2; // 로비 씬에서 다른 캐릭터와 겹치지 않기 위함
        DontDestroyOnLoad(playerObject);

        OnPlayerCharacterChanged?.Invoke();
        isFirstPlayerCreated = true;
    }

    private void OnSceneChanged(SceneType newScene)
    {
        if (newScene.IsReturnScene()) 
        { 
            Destroy(playerObject); 
            return; }
        if (newScene.IsPlayerDeactivatedScene()) 
        { 
            if(playerObject != null)
            {
                playerObject.SetActive(false);
            }
        }
    }
}
