using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RoomBoundary
{
    public GameObject room;
    public Vector3 bottomLeftCorner;
    public Vector3 topRightCorner;

    public RoomBoundary(GameObject room, Vector3 bottomLeftCorner, Vector3 topRightCorner)
    {
        this.room = room;
        this.bottomLeftCorner = bottomLeftCorner;
        this.topRightCorner = topRightCorner;
    }
}

public class DungeonHelper : MonoBehaviour
{
    private static DungeonHelper instance;
    public static DungeonHelper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameManagerObject = new GameObject("DungeonHelper");
                instance = gameManagerObject.AddComponent<DungeonHelper>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 생성된 각 방들의 경계값을 가진 List를 반환하는 함수
    public List<RoomBoundary> GetRoomBoundaries()
    {
        List<RoomBoundary> listToReturn = new List<RoomBoundary>();
        DungeonCreator dungeonCreator = DungeonFlowManager.Instance.DungeonCreator;
        if(dungeonCreator != null)
        {
            foreach(var room in dungeonCreator.generatedRooms)
            {
                listToReturn.Add(new RoomBoundary(
                    room,
                    room.GetComponent<Room>().BottomLeftCorner,
                    room.GetComponent<Room>().TopRightcorner));
            }
        }

        return listToReturn;
    }
}
