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
    public CharacterType CurrentCharacterType { get => currentCharacterType; }
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
    }

    void Update()
    {
        
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

    // 플레이어 사망 시, PlayerControllerBase에서 호출되는 함수
    public void PlayerDead()
    {
        // 스테이지 흐름 초기화
        DungeonFlowManager.Instance.ResetStages();
        // 플레이어를 엔딩 씬으로 이동
        // MoveScene(SceneType.EndScene, "EndScene");
        // PlayTimeStop();
    }
}
