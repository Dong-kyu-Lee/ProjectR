using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Speaker Profile")]
public class SpeakerProfile : ScriptableObject
{
    [Header("화자 정보")]
    public string characterName; // 이름
    //public Sprite portrait;      // 캐릭터 기본 사진
}