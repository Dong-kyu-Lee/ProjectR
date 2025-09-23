using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMapUI : MonoBehaviour
{
    [SerializeField]
    private int roomImageSize = 100; // 방 이미지의 크기
    [SerializeField]
    private int gap = 10; // 방 이미지 간의 간격
    private bool isBigMinimapActive = false; // 큰 미니맵 활성화 여부

    public GameObject mapParent; // 방 UI 이미지들이 배치될 부모 오브젝트
    public GameObject roomImagePrefab; // 방 UI 이미지 프리팹
    public GameObject bigMinimap;
    public List<GameObject> roomList = new List<GameObject>(); // 캔버스에 표시된 방 이미지들의 리스트

    private void Start()
    {
        Debug.Log("DungeonMapUI Start");
        DrawDungeonMap();
        DungeonFlowManager.Instance.GetCurrentStage().onDungeonReset?.AddListener(RemoveRoomImages);
    }

    private void OnDestroy()
    {
        DungeonFlowManager.Instance.GetCurrentStage().onDungeonReset?.RemoveListener(RemoveRoomImages);
    }

    void Update()
    {
        var roomInfos = DungeonFlowManager.Instance.GetCurrentStage().roomList;
        for (int i = 0; i < roomList.Count; i++)
        {
            switch(roomInfos[i].GetRoomState)
            {
                case RoomState.Default:
                    roomList[i].GetComponent<Image>().color = Color.white;
                    break;
                case RoomState.Start:
                    roomList[i].GetComponent<Image>().color = Color.yellow;
                    break;
                case RoomState.Cleared:
                    roomList[i].GetComponent<Image>().color = Color.gray;
                    break;
            }
        }
    }

    // 방들의 배치를 Canvas에 그리드 형태로 표시하는 메서드
    private void DrawDungeonMap()
    {
        Debug.Log("DrawDungeonMap");
        var roomInfos = DungeonFlowManager.Instance.GetCurrentStage().roomList;
        if(roomInfos == null || roomInfos.Count == 0)
        {
            Debug.LogError("roomInfos is null");
            return;
        }
        else if (roomInfos.Count == 0)
        {
            Debug.LogError("roomInfos count is 0");
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

    public void SwitchBigminimapAndFullmap()
    {
        if(isBigMinimapActive)
        {
            // 큰 미니맵을 비활성화
            isBigMinimapActive = false;
            bigMinimap.SetActive(false);
        }
        else
        {
            // 큰 미니맵을 활성화
            isBigMinimapActive = true;
            bigMinimap.SetActive(true);
        }
    }
}
