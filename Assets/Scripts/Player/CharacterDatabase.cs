using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video; // 스킬 예시 영상용

/// <summary>
/// 각 캐릭터에 대한 타입과 프리팹 정보를 저장한 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Scriptable Object/CharacterDatabase", order = 1)]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characterDataList = new List<CharacterData>();
}

[System.Serializable]
public struct CharacterData
{
    public CharacterType characterType;
    public GameObject characterPrefab;

    [Header("UI 이미지 리소스")]
    public Sprite portraitIcon;         // 인게임 좌상단 체력바용 초상화 (InGameUIManager 등에서 사용할 용도)
    public Sprite illustration;         // 로비 캐릭터 선택창용 일러스트 (데이터 없으면 비워둠)
    public Sprite skillIcon;            // 스킬 쿨타임 UI 및 캐릭터 선택창용 아이콘

    [Header("UI 텍스트 및 미디어")]
    public VideoClip skillPreviewVideo; // 로비 캐릭터 선택창용 스킬 영상 (데이터 없으면 비워둠)

    [TextArea(3, 5)]
    public string backgroundStory;      // 캐릭터 배경 스토리

    [TextArea(3, 5)]
    public string skillDescription;     // 추상화된 스킬 설명 (예: "자원을 소모하여 넓은 범위의 적을 타격합니다")
}

public enum CharacterType
{
    Bartender,
    Blacksmith,
}
