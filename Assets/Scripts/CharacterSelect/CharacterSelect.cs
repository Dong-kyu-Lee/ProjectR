using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        // BGM 재생
        SoundManager.Instance.Play("Sounds/BGM/LobbySceneBGM", Sound.Bgm);

        // 모든 캐릭터 씬에 생성
        for (int i = 0; i < mannequins.Length; i++)
        {
            CharacterType type = (CharacterType)i;
            // if(type == GameManager.Instance.CurrentCharacterType)
            //    continue; // 현재 플레이어 캐릭터는 생성하지 않음

            GameObject characterPrefab = PlayerManager.Instance.GetCharacterPrefab(type);
            if (characterPrefab != null)
            {
                characters[i] = Instantiate(characterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                characters[i].SetActive(false);
            }
            else
            {
                Debug.LogError($"Character prefab not found for type: {type}");
            }
        }

        // 현재 플레이어 오브젝트 설정(생성할 캐릭터 오브젝트, 타입, 위치)
        Vector3 spawnPosition; // 플레이어 오브젝트 생성 위치 결정(최초 로비 진입 여부에 따름)
        // 최초 로비 진입 연출용 위치. 소비 여부를 일회성 스토리 기록(json)으로 판단한다.
        // 프롤로그 완료 플래그와 분리해야 한다. 프롤로그는 로비 도착 전에 이미 완료 처리되기 때문이다.
        if (!StorySystem.Instance.IsSingleUseStoryCompleted(StoryID.FirstLobbyEntry))
        {
            spawnPosition = prologueSpawnPoint.position;
            StorySystem.Instance.CompleteSingleUseStory(StoryID.FirstLobbyEntry);
        }
        else { spawnPosition = mannequins[(int)GameManager.Instance.CurrentCharacterType].transform.position; }
        // 선택한 캐릭터를 CurrentPlayer로 설정하고 위치 지정
        PlayerManager.Instance.SetCurrentPlayer(
            characters[(int)PlayerManager.Instance.CurrentCharacterType],
            PlayerManager.Instance.CurrentCharacterType,
            spawnPosition
            );
        PlayerManager.Instance.CurrentPlayer.SetActive(true);

        SetMannequin(PlayerManager.Instance.CurrentCharacterType);
        vcam?.SetFollowTarget(PlayerManager.Instance.CurrentPlayer.transform);

        if (portraitHandler != null)
        {
            portraitHandler.ChangePortrait(PlayerManager.Instance.CurrentCharacterType);
        }
    }

    public void SelectCharacter(CharacterType type, Vector3 spawnPosition)
    {
        // 캐릭터 선택 시 해당 마네킹 비활성화
        SetMannequin(type);

        CharacterType prevCharacterType = PlayerManager.Instance.CurrentCharacterType;
        // 현재 플레이어 오브젝트 삭제하고 해당 캐릭터 오브젝트를 생성(DontDestroyOnLoad 해제를 위함)
        PlayerManager.Instance.SetCurrentPlayer(characters[(int)type], type, spawnPosition);
        // 이전 캐릭터 오브젝트 생성(SetCurrentPlayer 내부에서 이전 캐릭터 오브젝트 삭제)
        characters[(int)prevCharacterType] = Instantiate(
            PlayerManager.Instance.GetCharacterPrefab(prevCharacterType),
            new Vector3(0, 0, 0),
            Quaternion.identity
        );
        characters[(int)prevCharacterType].SetActive(false);

        if (portraitHandler != null)
        {
            portraitHandler.ChangePortrait(type);
        }

        PlayerManager.Instance.CurrentPlayer.SetActive(true);
        PlayerManager.Instance.CurrentPlayer.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
        // 카메라 설정
        vcam?.SetFollowTarget(PlayerManager.Instance.CurrentPlayer.transform);
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
