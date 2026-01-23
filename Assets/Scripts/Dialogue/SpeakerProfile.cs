using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Speaker Profile")]
public class SpeakerProfile : ScriptableObject
{
    public string characterName; // 화면에 표시될 이름
    public Sprite defaultPortrait; // 기본 초상화
}