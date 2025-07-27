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
    GameObject currentPlayer;
    [SerializeField]
    Mannequin[] mannequins;
    [SerializeField]
    CM_LobbyScene vcam;

    void Start()
    {
        // TODO : 이전에 선택한 캐릭터 데이터 있다면 그 캐릭터를 생성

        // 기본 캐릭터 생성
        SelectCharacter(mannequins[0].characterType, mannequins[0].transform.position);
    }

    public void SelectCharacter(CharacterType type, Vector3 spawnPosition)
    {
        // 마네킹 비활성화
        for (int i = 0; i < mannequins.Length; i++)
        {
            if(mannequins[i].characterType == type)
            {
                mannequins[i].gameObject.SetActive(false);
            }
            else
            {
                mannequins[i].gameObject.SetActive(true);
            }
        }

        // 이전 캐릭터 삭제
        if (currentPlayer != null) Destroy(currentPlayer);
        // 선택한 캐릭터 생성
        switch (type)
        {
            case CharacterType.Bartender:
                currentPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/Player Prefabs/Bartender2_1"), spawnPosition, transform.rotation);
                break;
            case CharacterType.Blacksmith:
                currentPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/Player Prefabs/Blacksmith2_1"), spawnPosition, transform.rotation);
                break;
        }
        GameManager.Instance.CurrentPlayer = currentPlayer;
        // 카메라 설정
        vcam?.SetFollowTarget(currentPlayer.transform);
    }
}
