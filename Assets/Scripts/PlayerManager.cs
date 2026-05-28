using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플레이어 오브젝트의 생성, 교체를 담당하는 매니저 클래스
/// </summary>
public class PlayerManager : MonoBehaviour
{
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
}
