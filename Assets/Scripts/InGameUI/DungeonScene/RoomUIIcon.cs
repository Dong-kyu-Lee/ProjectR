using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIIcon : MonoBehaviour
{
    [Serializable]
    public struct CharacterIcon
    {
        public Define.CharacterType type; // 아이콘 캐릭터 타입
        public Sprite sprite; // 방 아이콘으로 표시될 스프라이트
        public float width; // 아이콘 크기 차이로 인한 오프셋값: 너비 (기본 100)
        public float height; // 아이콘 크기 차이로 인한 오프셋값: 높이 (기본 100)
    }

    [Serializable]
    public struct RoomStateIcon
    {
        public RoomState roomState; // 이 아이콘이 나타내는 방 상태
        public Sprite sprite;
        public float width; // 아이콘 크기 차이로 인한 오프셋값: 너비 (기본 100)
        public float height; // 아이콘 크기 차이로 인한 오프셋값: 높이 (기본 100)
    }

    [SerializeField] Image roomIconObject;
    [SerializeField] CharacterIcon[] characterIcons;
    [SerializeField] RoomStateIcon[] roomStateIcons;

    // 방 상태에 맞는 아이콘으로 갱신하는 메서드
    // 진행중(Start)인 방은 현재 플레이중인 캐릭터 아이콘, 그 외에는 상태별 아이콘을 표시
    public void SetIcon(RoomState state)
    {
        if (roomIconObject == null) return;

        Sprite sprite = null;
        Vector2 size = Vector2.zero;

        if (state == RoomState.Start)
        {
            Define.CharacterType currentType = PlayerManager.Instance.CurrentCharacterType;
            foreach (CharacterIcon icon in characterIcons)
            {
                if (icon.type != currentType) continue;
                sprite = icon.sprite;
                size = new Vector2(icon.width, icon.height);
                break;
            }
        }
        else
        {
            foreach (RoomStateIcon icon in roomStateIcons)
            {
                if (icon.roomState != state) continue;
                sprite = icon.sprite;
                size = new Vector2(icon.width, icon.height);
                break;
            }
        }

        // 해당하는 아이콘이 없으면 빈 이미지(흰 사각형)가 보이지 않도록 숨김
        roomIconObject.enabled = sprite != null;
        if (sprite == null) return;

        roomIconObject.sprite = sprite;
        roomIconObject.rectTransform.sizeDelta = size;
    }
}
