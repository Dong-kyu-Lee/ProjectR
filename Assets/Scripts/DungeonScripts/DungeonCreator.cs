using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonCreator : MonoBehaviour
{
    [Header("Fixed Room Setting")]
    public int numberOfRooms;
    public int dungeonBoxCount;

    [Header("Needed Objects")]
    public RoomContainer roomContainer;
    [SerializeField]
    private Tilemap backgroundTilemap;
    [SerializeField]
    private Tilemap groundTilemap;
    [SerializeField]
    private Tilemap floatingTilemap;
    [SerializeField]
    private Tilemap decorationTilemap;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject roomInGamePrefab;
    private Dictionary<Vector3, GameObject> roomInGameDic = new Dictionary<Vector3, GameObject>();
    private List<Tuple<RoomNode, Room>> roomTupleList = new List<Tuple<RoomNode, Room>>();

    void Start()
    {
        if (roomContainer == null)
        {
            gameObject.AddComponent<RoomContainer>();
            roomContainer = GetComponent<RoomContainer>();
        }
        DungeonFlowManager.Instance.DungeonCreator = this;
        DungeonFlowManager.Instance.onDungeonCreatorReady.Invoke();

        if(DungeonTestHelper.Instance != null)
        {
            numberOfRooms = DungeonTestHelper.Instance.numberOfRooms;
            dungeonBoxCount = DungeonTestHelper.Instance.dungeonBoxCount;
        }
    }
    
    // 던전과 관련 요소를 씬에 생성하는 함수
    public void CreateFixedRoomDungeon(out Vector3 playerSpawnPosition, out Vector3 finishSpotPosition)
    {
        DungeonStructureGenerator dungeonStructure = new DungeonStructureGenerator(numberOfRooms);
        var roomNodes = dungeonStructure.GetDungeonStructure();

        playerSpawnPosition = new Vector3();
        finishSpotPosition = new Vector3();

        // 상자를 생성할 방 인덱스를 정하는 함수
        List<int> roomIndexForBoxes = GetRandomNumbers(roomNodes.Count, dungeonBoxCount);

        // 랜덤으로 방 프리팹을 선택해 방과 그 구성요소 생성
        for (int i = 0; i < roomNodes.Count; ++i)
        {
            Vector3Int generatePosition = new Vector3Int(40 * roomNodes[i].RoomPosition.x, 40 * roomNodes[i].RoomPosition.y, 0);

            Room currentRoom;

            if(DungeonTestHelper.Instance.testRoomPrefabs.Count != 0)
            {
                // 테스트용 방 프리팹이 있다면 해당 방 프리팹을 사용
                currentRoom = DungeonTestHelper.Instance.testRoomPrefabs[Random.Range(0, DungeonTestHelper.Instance.testRoomPrefabs.Count)].GetComponent<Room>();
            }
            else
            {
                // 일반적인 경우, RoomContainer에 있는 방 프리팹을 사용
                currentRoom = roomContainer.rooms[Random.Range(0, roomContainer.rooms.Count)].GetComponent<Room>();
            }
            DrawRoom(generatePosition, currentRoom, roomNodes[i].OpenNeededGate);
            roomTupleList.Add(new Tuple<RoomNode, Room>(roomNodes[i], currentRoom));

            // 문 그리기
            UpdateRoomInGame(generatePosition, roomNodes[i].OpenNeededGate);
            roomInGameDic[generatePosition].GetComponent<EnemyInRoom>().SetEnemyTilemap(currentRoom, generatePosition);
            // 맵의 동적 요소들 생성(ex. 움직이는 발판 등)
            roomInGameDic[generatePosition].GetComponent<RoomInGame>().SetDynamicElements(currentRoom.dynamicElements);
            // 상자 생성
            if(roomIndexForBoxes.Contains(i))
            {
                roomInGameDic[generatePosition].GetComponent<RoomInGame>().SetBoxObject(currentRoom.boxObject);
            }
            // DungeonFlowManager가 생성된 방을 추적할 수 있도록 방 정보를 추가함.
            DungeonFlowManager.Instance.AddRoomInGame(roomInGameDic[generatePosition].GetComponent<RoomInGame>());

            if (i == 0) playerSpawnPosition = generatePosition + currentRoom.playerSpawnPosition.position;
            else if (i == roomNodes.Count - 1) finishSpotPosition = generatePosition + currentRoom.finishSpotPosition.position;
        }

        UpdateWarpPosition();
    }

    // 문 오브젝트를 생성하고 갱신하는 함수
    private void UpdateRoomInGame(Vector3 doorPosition, bool[] openNeededGate)
    {
        // 2스테이지 시작의 경우
        if (roomInGameDic.Count == numberOfRooms)
        {
            roomInGameDic[doorPosition].GetComponent<RoomInGame>().ResetRoomState();
            roomInGameDic[doorPosition].GetComponent<Gate>().SetUsableDoors(openNeededGate);
        }
        else
        {
            roomInGameDic.Add(doorPosition, Instantiate(roomInGamePrefab, doorPosition, transform.rotation, grid.transform));
            roomInGameDic[doorPosition].GetComponent<Gate>().SetUsableDoors(openNeededGate);
        }
    }

    // 워프가 생성되어야 할 방을 선정하고 해당 방의 RoomInGame 을 수정
    private void UpdateWarpPosition()
    {
        for (int i = 0; i < roomTupleList.Count - 1; ++i)
        {
            Vector2Int currentNodePosition = roomTupleList[i].Item1.RoomPosition;
            Vector2Int nextNodePosition = roomTupleList[i + 1].Item1.RoomPosition;
            if (currentNodePosition.y != nextNodePosition.y)
            {
                Vector3 roomPosition = new Vector3Int(40 * currentNodePosition.x, 40 * currentNodePosition.y, 0);
                Vector3 nextRoomPosition = new Vector3Int(40 * nextNodePosition.x, 40 * nextNodePosition.y, 0);

                Vector3 warpPosition = roomPosition + roomTupleList[i].Item2.warpPosition.position;
                Vector3 playerWarpPosition = nextRoomPosition + roomTupleList[i + 1].Item2.playerSpawnPosition.position;
                roomInGameDic[roomPosition].GetComponent<Gate>().CreateWarpObject(warpPosition, playerWarpPosition);
            }
        }
    }

    private List<int> GetRandomNumbers(int roomCount, int boxCount)
    {
        // 예외 처리: M이 N보다 크면 안 됨
        if (boxCount > roomCount)
        {
            Debug.LogError("M은 N보다 클 수 없습니다.");
            return null;
        }

        // 전체 숫자 리스트 생성 (0부터 N까지)
        List<int> numbers = new List<int>();
        for (int i = 0; i <= roomCount; i++)
        {
            numbers.Add(i);
        }

        // 랜덤으로 M개의 숫자를 선택
        List<int> randomNumbers = new List<int>();
        for (int i = 0; i < boxCount; i++)
        {
            int randomIndex = Random.Range(0, numbers.Count); // 랜덤 인덱스 선택
            randomNumbers.Add(numbers[randomIndex]);          // 랜덤 숫자 추가
            numbers.RemoveAt(randomIndex);                    // 중복 방지를 위해 제거
        }

        return randomNumbers;
    }

    // 던전 방 프리팹을 타일맵에 그리는 함수
    private void DrawRoom(Vector3Int roomPosition, Room room, bool[] openNeededGate)
    {
        // Background Tilemap 그리기
        Tilemap bTilemap = room.backgroundTilemap;
        for(int i = 0; i < bTilemap.size.y; ++i)
        {
            for(int j = 0; j < bTilemap.size.x; ++j)
            {
                backgroundTilemap.SetTile(new Vector3Int(roomPosition.x + j, roomPosition.y + i, 0),
                    bTilemap.GetTile(new Vector3Int(j, i, 0)));
            }
        }
        // Ground Tilemap 그리기
        Tilemap gTilemap = room.groundTilemap;
        for (int i = 0; i < gTilemap.size.y; ++i)
        {
            for (int j = 0; j < gTilemap.size.x; ++j)
            {
                groundTilemap.SetTile(new Vector3Int(roomPosition.x + j, roomPosition.y + i, 0),
                    gTilemap.GetTile(new Vector3Int(j, i, 0)));
            }
        }
        // Floating Tilemap 그리기
        Tilemap fTilemap = room.floatingTilemap;
        for (int i = 0; i < fTilemap.size.y; ++i)
        {
            for (int j = 0; j < fTilemap.size.x; ++j)
            {
                floatingTilemap.SetTile(new Vector3Int(roomPosition.x + j, roomPosition.y + i, 0),
                    fTilemap.GetTile(new Vector3Int(j, i, 0)));
            }
        }
        // Decoration Tilemap 그리기
        Tilemap dTilemap = room.decorationTilemap;
        for (int i = 0; i < dTilemap.size.y; ++i)
        {
            for (int j = 0; j < dTilemap.size.x; ++j)
            {
                decorationTilemap.SetTile(new Vector3Int(roomPosition.x + j, roomPosition.y + i, 0),
                    dTilemap.GetTile(new Vector3Int(j, i, 0)));
            }
        }

        // 통로 열기
        if (openNeededGate[1] == true) // 오른쪽
        {
            for (int i = 0; i < 8; ++i)
            {
                groundTilemap.SetTile(roomPosition + new Vector3Int(39, 16 + i, 0), null);
            }
        }
        if (openNeededGate[3] == true) // 왼쪽
        {
            for (int i = 0; i < 8; ++i)
            {
                groundTilemap.SetTile(roomPosition + new Vector3Int(0, 16 + i, 0), null);
            }
        }
    }

    public void RemoveAllRooms()
    {
        backgroundTilemap.ClearAllTiles();
        groundTilemap.ClearAllTiles();
        floatingTilemap.ClearAllTiles();
        decorationTilemap.ClearAllTiles();

        foreach (var room in roomInGameDic)
        {
            Destroy(room.Value);
        }
        roomInGameDic.Clear();
        roomTupleList.Clear();
    }
}
