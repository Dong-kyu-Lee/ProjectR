using System;
using System.Collections.Generic;
using UnityEngine;

// 스테이지 흐름 그래프의 노드 종류. 각 타입은 Define.SceneType과 1:1로 대응한다.
public enum StageNodeType
{
    NormalDungeon, MiddleBoss, FinalBoss, Shop,
}

// 나가는 간선이 2개 이상일 때 다음 노드를 고르는 방식
public enum StageBranchMode
{
    First,        // 첫 번째 간선만 사용 (선형 흐름)
    Random,       // weight 기반 랜덤 선택
    PlayerChoice, // 플레이어 선택 (선택 UI는 추후 구현. 선택값이 없으면 첫 번째 간선 사용)
}

// 노드 간 연결(방향 간선)
[Serializable]
public class StageEdge
{
    [Tooltip("이동할 노드의 id")]
    public string targetNodeId;

    [Tooltip("Random 모드에서 사용하는 가중치")]
    [Min(0)] public float weight = 1f;

    [Tooltip("PlayerChoice 모드에서 플레이어에게 보여줄 선택지 이름")]
    public string choiceLabel;

    public StageEdge() { }

    public StageEdge(string targetNodeId)
    {
        this.targetNodeId = targetNodeId;
    }
}

// 스테이지 흐름 그래프의 노드(구역) 하나
[Serializable]
public class StageNode
{
    [Tooltip("그래프 안에서 유일한 id. 예) normal_1, mid_boss")]
    public string id;
    public StageNodeType type;

    [Tooltip("비우면 타입별 기본 씬을 사용 (DungeonGenerate / TempMiddleBoss / TempFinalBoss / ShopScene)")]
    public string sceneName;

    [Header("Normal Dungeon")]
    [Tooltip("0 이하이면 StageData의 numberOfRooms 사용")]
    public int numberOfRoomsOverride = 0;
    [Tooltip("음수이면 StageData의 dungeonBoxCount 사용")]
    public int dungeonBoxCountOverride = -1;

    [Header("Boss")]
    [Tooltip("보스 처치 후 컷씬 재생 여부")]
    public bool playStory = true;
    public StoryID storyID;

    [Header("Transition")]
    public StageBranchMode branchMode = StageBranchMode.First;
    [Tooltip("비어 있으면 이 노드를 끝으로 스테이지가 종료된다")]
    public List<StageEdge> edges = new List<StageEdge>();

    public const string DefaultDungeonScene = "DungeonGenerate";
    public const string DefaultMiddleBossScene = "TempMiddleBoss";
    public const string DefaultFinalBossScene = "TempFinalBoss";
    public const string DefaultShopScene = "ShopScene";

    public bool IsTerminal { get => edges == null || edges.Count == 0; }

    public string SceneName
    {
        get
        {
            if (string.IsNullOrEmpty(sceneName) == false) return sceneName;
            switch (type)
            {
                case StageNodeType.MiddleBoss: return DefaultMiddleBossScene;
                case StageNodeType.FinalBoss: return DefaultFinalBossScene;
                case StageNodeType.Shop: return DefaultShopScene;
                default: return DefaultDungeonScene;
            }
        }
    }

    public Define.SceneType SceneType
    {
        get
        {
            switch (type)
            {
                case StageNodeType.MiddleBoss: return Define.SceneType.MiddleBoss;
                case StageNodeType.FinalBoss: return Define.SceneType.FinalBossScene;
                case StageNodeType.Shop: return Define.SceneType.Shop;
                default: return Define.SceneType.Normal;
            }
        }
    }

    public bool IsBoss { get => type == StageNodeType.MiddleBoss || type == StageNodeType.FinalBoss; }

    // 노드 설정값이 있으면 사용하고, 없으면 StageData의 기본값을 사용
    public int GetNumberOfRooms(StageData stageData)
    {
        return numberOfRoomsOverride > 0 ? numberOfRoomsOverride : stageData.numberOfRooms;
    }

    public int GetDungeonBoxCount(StageData stageData)
    {
        int boxCount = dungeonBoxCountOverride >= 0 ? dungeonBoxCountOverride : stageData.dungeonBoxCount;
        return Mathf.Clamp(boxCount, 0, GetNumberOfRooms(stageData));
    }

    public StageNode() { }

    public StageNode(string id, StageNodeType type, params string[] nextNodeIds)
    {
        this.id = id;
        this.type = type;
        foreach (string nextId in nextNodeIds)
            edges.Add(new StageEdge(nextId));
    }
}
