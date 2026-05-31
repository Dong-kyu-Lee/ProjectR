using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public enum CharacterType
{
    Bartender,
    Blacksmith,
}
