using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonRow;
    public int dungeonColumn;
    public GameObject grid;
    public RoomContainer roomContainer;

    public List<GameObject> generatedRooms = new List<GameObject>();

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

    public void RemoveAllRooms()
    {
        for(int i = 0; i < generatedRooms.Count; ++i)
        {
            Destroy(generatedRooms[i]);
        }
        generatedRooms.Clear();
    }
}
