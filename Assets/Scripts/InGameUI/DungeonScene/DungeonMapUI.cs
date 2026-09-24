using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 미니맵에서 던전 전체 구조를 보여주는 UI를 갱신하는 클래스
public class DungeonMapUI : MonoBehaviour
{
    [SerializeField]
    private int roomImageSize = 100; // 방 이미지의 크기
    [SerializeField]
    private int gap = 10; // 방 이미지 간의 간격

    public GameObject mapParent; // 방 UI 이미지들이 배치될 부모 오브젝트
    public GameObject roomImagePrefab; // 방 UI 이미지 프리팹
    public List<GameObject> roomList = new List<GameObject>(); // 캔버스에 표시된 방 이미지들의 리스트

    private void Start()
    {
        Debug.Log("DungeonMapUI Start");
        DrawDungeonMap();

        // [수정] null 참조 방지를 위한 방어적 코드 추가
        if (DungeonFlowManager.Instance != null && DungeonFlowManager.Instance.GetCurrentStage() != null)
        {
            DungeonFlowManager.Instance.GetCurrentStage().onDungeonReset?.AddListener(RemoveRoomImages);
        }
    }

    private void OnDestroy()
    {
        if (DungeonFlowManager.Instance == null || DungeonFlowManager.Instance.GetCurrentStage() == null)
            return;

        DungeonFlowManager.Instance.GetCurrentStage().onDungeonReset?.RemoveListener(RemoveRoomImages);
    }

    // [수정/제거] Update() 내부에서 매 프레임 모든 방 상태를 순회하며 색상을 갱신하던 로직 완전 제거

    // 방들의 배치를 Canvas에 그리드 형태로 표시하는 메서드
    private void DrawDungeonMap()
    {
        Debug.Log("DrawDungeonMap");
        var roomInfos = DungeonFlowManager.Instance.GetCurrentStage().roomList;

        if (roomInfos == null || roomInfos.Count == 0)
        {
            Debug.LogError("roomInfos is null or count is 0");
            return;
        }

        Vector2 bottomLeft = new Vector2(1000f, 1000f); // 가장 왼쪽 아래 방
        Vector2 topRight = new Vector2(-1000f, -1000f); // 가장 오른쪽 위 방

        // 방들의 배치를 Canvas에 그리드 형태로 표시하는 로직을 구현합니다.
        // 각 방의 위치와 상태를 기반으로 UI 요소를 생성하고 배치합니다.
        Vector2 tmpRoomPos;
        for (int i = 0; i < roomInfos.Count; i++)
        {
            Vector3 pos = roomInfos[i].gameObject.transform.position;
            // 방의 실제 좌표 -> 노드 상 좌표로 변환 (ex. (40, 80) -> (1, 2))
            tmpRoomPos = new Vector2(pos.x / 40, pos.y / 40);
            // 노드 상 좌표 -> 방 UI 이미지 좌표로 변환
            Vector2 uiRoomPos = new Vector2(tmpRoomPos.x * (roomImageSize + gap), tmpRoomPos.y * (roomImageSize + gap));
            // 방 UI 이미지 생성
            GameObject roomImage = Instantiate(roomImagePrefab, mapParent.transform);
            roomImage.GetComponent<RectTransform>().anchoredPosition = uiRoomPos;

            roomList.Add(roomImage);

            // 방 UI 위치의 가장 왼쪽 아래와 오른쪽 위 좌표를 구합니다.
            bottomLeft = Vector2.Min(bottomLeft, uiRoomPos);
            topRight = Vector2.Max(topRight, uiRoomPos);

            // [추가] 1. 방을 처음 그릴 때 초기 색상 1회 설정
            UpdateRoomColor(i, roomInfos[i].GetRoomState);

            // [추가] 2. 해당 방의 상태 변화 이벤트 구독
            // 클로저(Closure) 현상으로 인해 i값이 잘못 참조되는 것을 막기 위해 지역 변수 index에 복사해서 넘겨줍니다.
            int index = i;
            roomInfos[i].OnRoomStateChanged += (newState) => UpdateRoomColor(index, newState);
        }

        // 방 UI 이미지를 map parent 가운데로 배치
        Vector2 middle = (bottomLeft + topRight) / 2;
        for (int i = 0; i < roomList.Count; i++)
        {
            Vector2 uiRoomPos = roomList[i].GetComponent<RectTransform>().anchoredPosition;
            uiRoomPos -= middle;
            roomList[i].GetComponent<RectTransform>().anchoredPosition = uiRoomPos;
        }
    }

    // [추가] 전달받은 인덱스와 상태에 맞춰서 특정 UI 한 개의 색상만 갱신하는 메서드
    public void UpdateRoomColor(int index, RoomState state)
    {
        if (index < 0 || index >= roomList.Count) return;

        Image roomImg = roomList[index].GetComponent<Image>();
        if (roomImg == null) return;

        switch (state)
        {
            case RoomState.Default:
                roomImg.color = Color.white;
                break;
            case RoomState.Start:
                roomImg.color = Color.yellow;
                break;
            case RoomState.Cleared:
                roomImg.color = Color.gray;
                break;
        }
    }

    // 던전이 갱신되었을 때 방 UI 이미지들을 제거하는 메서드
    private void RemoveRoomImages()
    {
        Debug.Log("RemoveRoomImages");
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }
        roomList.Clear();
        DrawDungeonMap();
    }
}