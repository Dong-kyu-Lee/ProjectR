using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이지 데이터를 저장하는 ScriptableObject 클래스
[CreateAssetMenu(fileName = "StageData", menuName = "Stage Assets/StageData", order = 1)]
public class StageData : ScriptableObject
{
    [SerializeField]
    private string stageName;
    public string middleBossSceneName;
    public string finalBossSceneName;
    public List<GameObject> roomPrefabs;

    [Tooltip("스테이지 배경 이미지 프리팹(BGImageMove). 배경 타일을 사용하는 스테이지는 비워 둘 것")]
    public GameObject backgroundImagePrefab;

    [Header("Dungeon Generate Setting")]
    [Min(1)] public int numberOfRooms = 3;
    [Min(0)] public int dungeonBoxCount = 2;

    private void OnValidate()
    {
        // 상자 개수가 방 개수보다 많으면 상자를 배치할 방을 정할 수 없으므로 제한
        dungeonBoxCount = Mathf.Clamp(dungeonBoxCount, 0, numberOfRooms);
    }
}
