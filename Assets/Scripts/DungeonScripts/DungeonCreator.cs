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
    [Header("Random Room Setting")]
    public int roomCount;
    public int jumpHeight, dashWidth;
    public int roomWidthMin, roomHeightMin;
    public int roomWidthMax, roomHeightMax;
    public int floatingTileWidthMin, floatingTileWidthMax;

    [Header("For Random Dungeon Test")]
    public Tilemap tilemap;
    public TileBase floatingTile;
    public TileBase boundTile;

    [Header("Needed Objects")]
    public GameObject grid;
    public RoomContainer roomContainer;
    public List<GameObject> generatedRooms = new List<GameObject>();

    void Start()
    {
        /*if (roomContainer == null)
        {
            gameObject.AddComponent<RoomContainer>();
            roomContainer = GetComponent<RoomContainer>();
        }
        DungeonFlowManager.Instance.DungeonCreator = this;
        DungeonFlowManager.Instance.onDungeonCreatorReady.Invoke();*/
    }
    
    // 선택된 던전 조각들을 Instantiate하는 함수
    public void CreateFixedRoomDungeon(out Vector3 playerSpawnPosition, out Vector3 finishSpotPosition)
    {
        FixedRoomDungeonGenerator dungeonGenerator = new FixedRoomDungeonGenerator(dungeonRow, dungeonColumn);
        var listOfRoomNodes = dungeonGenerator.CreateRoomNodes();

        // 조건에 맞는 방들을 랜덤으로 선택해 생성
        foreach( RoomNode roomNode in listOfRoomNodes)
        {
            Vector3 generatePosition = new Vector3(20 * roomNode.RoomPosition.x, 20 * roomNode.RoomPosition.y, 0);

            var usableRooms = roomContainer.GetRooms(roomNode.OpenNeededGate);

            generatedRooms.Add(Instantiate(usableRooms[Random.Range(0, usableRooms.Count)], generatePosition,
                transform.rotation, grid.transform));
            // 방의 통로를 만드는 작업
            generatedRooms[generatedRooms.Count - 1].GetComponent<Room>().OpenGateTile(roomNode.OpenNeededGate);
            // 해당 방의 경계값을 저장
            generatedRooms[generatedRooms.Count - 1].GetComponent<Room>().
                SetRoomBoundary(generatePosition, generatePosition + new Vector3(20, 20, 0));
        }

        // 스테이지의 플레이어 스폰 지점을 결정
        playerSpawnPosition = generatedRooms[0].GetComponent<Room>().playerSpawnPosition.position;
        // 스테이지의 클리어 지점 결정 후 활성화
        finishSpotPosition = generatedRooms[Random.Range(1, generatedRooms.Count)].GetComponent<Room>().finishSpotPosition.position;
    }

    
    // 랜덤 던전을 생성하는 함수
    public void CreateRandomRoomDungeon()
    {
        tilemap.ClearAllTiles();

        // 랜덤 던전 방 구조를 생성
        RandomRoomGenerator dungeonGenerator = new RandomRoomGenerator(roomCount, jumpHeight, dashWidth);
        var listOfRoom = dungeonGenerator.CalculateDungeon(roomCount, roomWidthMin, roomHeightMin, roomWidthMax, roomHeightMax);

        // 발판 타일 생성
        FloatingTileGenerator floatingTileGenerator = new FloatingTileGenerator(jumpHeight, dashWidth, floatingTileWidthMin, floatingTileWidthMax);
        var listOfFloatingTile = floatingTileGenerator.GenerateFloatingTile(listOfRoom);

        // 벽 타일을 그릴 위치
        List<Vector3Int> boundTilePositions = new List<Vector3Int>();
        // 벽 타일이 다른 방과 겹치는 위치
        List<Vector3Int> overlappedPositions = new List<Vector3Int>();
        int overlappedPosSize = 0;

        // 방 타일 그리기
        foreach (var room in listOfRoom)
        {
            for (int i = room.BottomLeft.y; i <= room.TopRight.y; ++i)
            {
                for (int j = room.BottomLeft.x; j <= room.TopRight.x; ++j)
                {
                    if (i == room.BottomLeft.y || i == room.TopRight.y ||
                        j == room.BottomLeft.x || j == room.TopRight.x)
                    {
                        Vector3Int point = new Vector3Int(j, i, 0);
                        if (boundTilePositions.Contains(point))
                        {
                            overlappedPositions.Add(point);
                        }
                        else
                        {
                            boundTilePositions.Add(point);
                        }
                    }
                }
            }

            // 겹치는 벽 타일들 중, 첫번째와 마지막 벽 타일은 맵의 매끄러움을 위해 빼준다.
            if (overlappedPositions.Count > 0)
            {
                overlappedPositions.RemoveAt(overlappedPosSize);
                overlappedPositions.RemoveAt(overlappedPositions.Count - 1);
                overlappedPosSize = overlappedPositions.Count;
            }
        }
        // 벽 타일 그리기
        foreach(var point in boundTilePositions)
        {
            tilemap.SetTile(point, boundTile);
        }
        // 겹치는 벽 타일 지우기
        foreach(var point in overlappedPositions)
        {
            tilemap.SetTile(point, null);
        }

        // 발판 타일 그리기
        foreach (var tile in listOfFloatingTile)
        {
            GameObject newTilemapObj = new GameObject("FloatingTile");
            newTilemapObj.transform.parent = grid.transform;
            Tilemap newTilemap = newTilemapObj.AddComponent<Tilemap>();
            newTilemap.tileAnchor = new Vector3(0, 0, 0);
            newTilemapObj.AddComponent<TilemapRenderer>();
            for(int i = tile.left.x; i <= tile.right.x; ++i)
            {
                newTilemap.SetTile(new Vector3Int(i, tile.left.y), floatingTile);
            }
        }
    }

    public void RemoveAllRooms()
    {
        for(int i = 0; i < generatedRooms.Count; ++i)
        {
            Destroy(generatedRooms[i]);
        }
        generatedRooms.Clear();
    }
}
