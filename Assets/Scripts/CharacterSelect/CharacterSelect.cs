using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Bartender,
    Blacksmith,
}

public class CharacterSelect : MonoBehaviour
{
    // Mannequin과 CharacterType에 해당하는 캐릭터 종류의 순서가 같아야 함.
    GameObject[] characters = new GameObject[System.Enum.GetValues(typeof(CharacterType)).Length];
    [SerializeField]
    Mannequin[] mannequins;
    [SerializeField]
    CM_LobbyScene vcam;
    [SerializeField]
    Transform prologueSpawnPoint;
    //UI 초상화
    [SerializeField] private CharacterPortraitHandler portraitHandler;

    private void Awake()
    {
        if (GameManager.Instance.isFirstPlayerCreated == false)
        {
            // 최초 플레이어 생성
            GameManager.Instance.CreateFirstPlayer();
            GameManager.Instance.isFirstPlayerCreated = true;
        }
    }

    void Start()
    {
        // BGM 재생
        GameManager.Sound.Play("Sounds/BGM/LobbySceneBGM", Sound.Bgm);

        // 캐릭터 생성
        for (int i = 0; i < mannequins.Length; i++)
        {
            CharacterType type = (CharacterType)i;
            // if(type == GameManager.Instance.CurrentCharacterType)
            //    continue; // 현재 플레이어 캐릭터는 생성하지 않음

            string path = GameManager.Instance.characterPrefabPaths[type];
            GameObject characterPrefab = Resources.Load<GameObject>(path);
            if (characterPrefab != null)
            {
                characters[i] = Instantiate(characterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                characters[i].SetActive(false);
            }
            else
            {
                Debug.LogError($"Character prefab not found at path: {path}");
            }
        }
        // 플레이어 오브젝트 생성 위치 결정(프롤로그 완료 여부에 따름)
        Vector3 spawnPosition;
        if (GameManager.Instance.prologue.hasSeenPrologue == false) { 
            spawnPosition = prologueSpawnPoint.position;
            GameManager.Instance.prologue.EndPrologue();
        }
        else { spawnPosition = mannequins[(int)GameManager.Instance.CurrentCharacterType].transform.position; }
        // 현재 플레이어 오브젝트 설정(생성할 캐릭터 오브젝트, 타입, 위치)
        GameManager.Instance.SetCurrentPlayer(
            characters[(int)GameManager.Instance.CurrentCharacterType],
            GameManager.Instance.CurrentCharacterType,
            spawnPosition
            );
        GameManager.Instance.CurrentPlayer.SetActive(true);

        SetMannequin(GameManager.Instance.CurrentCharacterType);
        vcam?.SetFollowTarget(GameManager.Instance.CurrentPlayer.transform);

        if (portraitHandler != null)
        {
            portraitHandler.ChangePortrait(GameManager.Instance.CurrentCharacterType);
        }
    }

    public void SelectCharacter(CharacterType type, Vector3 spawnPosition)
    {
        // 캐릭터 선택 시 해당 마네킹 비활성화
        SetMannequin(type);

        CharacterType prevCharacterType = GameManager.Instance.CurrentCharacterType;
        // 현재 플레이어 오브젝트 삭제하고 해당 캐릭터 오브젝트를 생성(DontDestroyOnLoad 해제를 위함)
        GameManager.Instance.SetCurrentPlayer(characters[(int)type], type, spawnPosition);
        // 이전 캐릭터 오브젝트 생성(SetCurrentPlayer 내부에서 이전 캐릭터 오브젝트 삭제)
        characters[(int)prevCharacterType] = Instantiate(
            Resources.Load<GameObject>(GameManager.Instance.characterPrefabPaths[prevCharacterType])
            , new Vector3(0, 0, 0), Quaternion.identity);
        characters[(int)prevCharacterType].SetActive(false);

        if (portraitHandler != null)
        {
            portraitHandler.ChangePortrait(type);
        }

        GameManager.Instance.CurrentPlayer.SetActive(true);
        // 카메라 설정
        vcam?.SetFollowTarget(GameManager.Instance.CurrentPlayer.transform);
    }

    private void SetMannequin(CharacterType type)
    {
        for (int i = 0; i < mannequins.Length; i++)
        {
            if (mannequins[i].characterType == type)
            {
                mannequins[i].gameObject.SetActive(false);
            }
            else
            {
                mannequins[i].gameObject.SetActive(true);
            }
        }
    }
}
