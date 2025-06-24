using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 던전 테스트를 위한 헬퍼 클래스
public class DungeonTestHelper : MonoBehaviour
{
    [Header("Dungeon Test Settings")]
    public int numberOfRooms = 3;
    public int dungeonBoxCount = 2;

    [Header("UI Test")]
    public GameObject testUI; // 이곳에 던전 씬에서 테스트할 UI를 할당하세요.

    [Header("Dungeon Object Test")]
    public List<GameObject> testRoomPrefabs; // 이곳에 던전 씬에서 테스트할 방 프리팹을 할당하세요.

    private static DungeonTestHelper instance;
    public static DungeonTestHelper Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("DungeonTestHelper instance not found in the scene. Please add it to the scene.");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            testRoomPrefabs = new List<GameObject>();
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
