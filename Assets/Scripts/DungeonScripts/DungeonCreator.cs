using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonCreator : MonoBehaviour
{
    [Header("Fixed Room Setting")]
    public int dungeonRow;
    public int dungeonColumn;

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
    }
    
    // 던전과 관련 요소를 씬에 생성하는 함수
    public void CreateFixedRoomDungeon(out Vector3 playerSpawnPosition, out Vector3 finishSpotPosition)
    {
        DungeonStructureGenerator dungeonStructure = new DungeonStructureGenerator(dungeonRow, dungeonColumn);
        var roomNodes = dungeonStructure.GetDungeonStructure();

        playerSpawnPosition = new Vector3();
        finishSpotPosition = new Vector3();

        // 랜덤으로 방 프리팹을 선택해 방과 그 구성요소 생성
        for(int i = 0; i < roomNodes.Count; ++i)
        {
            Vector3Int generatePosition = new Vector3Int(40 * roomNodes[i].RoomPosition.x, 40 * roomNodes[i].RoomPosition.y, 0);
            var usableRooms = roomContainer.GetRooms(roomNodes[i].OpenNeededGate);
            Room currentRoom = usableRooms[Random.Range(0, usableRooms.Count)].GetComponent<Room>();
            DrawRoom(generatePosition, currentRoom, roomNodes[i].OpenNeededGate);
            // 문 그리기
            UpdateRoomInGame(generatePosition, roomNodes[i].OpenNeededGate);
            roomInGameDic[generatePosition].GetComponent<EnemyInRoom>().SetEnemyTilemap(currentRoom, generatePosition);

            roomTupleList.Add(new Tuple<RoomNode, Room>(roomNodes[i], currentRoom));

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
        if (roomInGameDic.Count == dungeonColumn * dungeonRow)
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
        if (openNeededGate[0] == true) // 위
        {
            for (int i = 0; i < 8; ++i)
            {
                groundTilemap.SetTile(roomPosition + new Vector3Int(16 + i, 39, 0), null);
            }
        }
        if (openNeededGate[1] == true) // 오른쪽
        {
            for (int i = 0; i < 8; ++i)
            {
                groundTilemap.SetTile(roomPosition + new Vector3Int(39, 16 + i, 0), null);
            }
        }
        if (openNeededGate[2] == true) // 아래
        {
            for (int i = 0; i < 8; ++i)
            {
                groundTilemap.SetTile(roomPosition + new Vector3Int(16 + i, 0, 0), null);
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
    }
}
