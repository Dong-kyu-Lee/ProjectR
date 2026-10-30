using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이지 데이터를 저장하는 ScriptableObject 클래스
[CreateAssetMenu(fileName = "StageData", menuName = "Stage Assets/StageData", order = 1)]
public class StageData : ScriptableObject
{
    [SerializeField]
    private string stageName;
    public List<GameObject> roomPrefabs;

    [Tooltip("스테이지 배경 이미지 프리팹(BGImageMove). 배경 타일을 사용하는 스테이지는 비워 둘 것")]
    public GameObject backgroundImagePrefab;

    [Header("Dungeon Generate Setting")]
    [Min(1)] public int numberOfRooms = 3;
    [Min(0)] public int dungeonBoxCount = 2;

    [Header("Stage Flow Graph")]
    [Tooltip("스테이지가 시작될 노드의 id")]
    public string startNodeId;
    [Tooltip("스테이지 흐름 그래프. 비어 있으면 기본 흐름(일반-일반-중간보스-일반-상점-최종보스)을 사용")]
    public List<StageNode> nodes = new List<StageNode>();

    private void OnValidate()
    {
        // 상자 개수가 방 개수보다 많으면 상자를 배치할 방을 정할 수 없으므로 제한
        dungeonBoxCount = Mathf.Clamp(dungeonBoxCount, 0, numberOfRooms);

        foreach (string error in StageGraph.Validate(startNodeId, nodes))
            Debug.LogWarning($"[{name}] 스테이지 그래프 오류: {error}", this);
    }

    // 인스펙터 우클릭 메뉴: 기본 흐름을 노드 목록으로 채워 편집의 시작점으로 사용
    [ContextMenu("Fill Default Stage Flow")]
    private void FillDefaultStageFlow()
    {
        nodes = StageGraph.CreateDefaultNodes();
        startNodeId = nodes[0].id;
    }
}
