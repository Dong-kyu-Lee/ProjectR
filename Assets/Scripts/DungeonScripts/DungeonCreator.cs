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
    public GameObject[] roomPrefabs;

    [SerializeField]
    private List<GameObject> generatedRooms;

    void Start()
    {
        CreateFixedRoomDungeon();
        generatedRooms = new List<GameObject>();
    }
    
    // 선택된 던전 조각들을 Instantiate하는 함수
    private void CreateFixedRoomDungeon()
    {
        FixedRoomDungeonGenerator dungeonGenerator = new FixedRoomDungeonGenerator(dungeonRow, dungeonColumn);
        var listOfRooms = dungeonGenerator.SelectRooms();

        // 조건에 맞는 방들을 랜덤으로 선택해 생성
        foreach( var room in listOfRooms )
        {
            Vector3 generatePosition = new Vector3(20 * room.RoomPosition.x, 20 * room.RoomPosition.y, 0);

            // TODO : room 프리펩 여러 종류 구현 후 배치 조건에 따라 if문 생성

            generatedRooms.Add(Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], generatePosition,
                transform.rotation, grid.transform));
            // 방의 Gate를 닫는 작업
            generatedRooms[generatedRooms.Count - 1].GetComponent<Room>().OpenGateTile(room.OpenNeededGate);
        }
    }
}
