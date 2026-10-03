using System.Collections.Generic;
using UnityEngine;

// ScriptableObject에서 Enum과 프리팹을 1:1 매핑하기 위한 직렬화 구조체
[System.Serializable]
public struct NormalEnemyPrefabMapping
{
    public Define.NormalEnemyType enemyType;
    public GameObject prefab;
}

[System.Serializable]
public struct MiddleBossPrefabMapping
{
    public Define.MiddleBossEnemyType enemyType;
    public GameObject prefab;
}

[System.Serializable]
public struct FinalBossPrefabMapping
{
    public Define.FinalBossEnemyType enemyType;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "EnemyPrefabPath", menuName = "Scriptable Object/Enemy Prefab Path", order = int.MaxValue)]
public class EnemyPrefabPath : ScriptableObject
{
    [Header("Normal Enemy Mappings")]
    [SerializeField] private List<NormalEnemyPrefabMapping> normalEnemyMappings = new List<NormalEnemyPrefabMapping>();
    
    [Header("Boss Enemy Mappings")]
    [SerializeField] private List<MiddleBossPrefabMapping> middleBossMappings = new List<MiddleBossPrefabMapping>();
    [SerializeField] private List<FinalBossPrefabMapping> finalBossMappings = new List<FinalBossPrefabMapping>();

    // 런타임 O(1) 탐색을 위한 캐싱 딕셔너리
    private Dictionary<Define.NormalEnemyType, GameObject> _normalEnemyDict;
    private Dictionary<Define.MiddleBossEnemyType, GameObject> _middleBossDict;
    private Dictionary<Define.FinalBossEnemyType, GameObject> _finalBossDict;

    private void OnEnable()
    {
        InitializeDictionaries();
    }

    // 리스트 데이터를 딕셔너리로 변환하여 초기화
    public void InitializeDictionaries()
    {
        _normalEnemyDict = new Dictionary<Define.NormalEnemyType, GameObject>();
        foreach (var mapping in normalEnemyMappings)
        {
            if (mapping.prefab != null && !_normalEnemyDict.ContainsKey(mapping.enemyType))
            {
                _normalEnemyDict.Add(mapping.enemyType, mapping.prefab);
            }
        }

        _middleBossDict = new Dictionary<Define.MiddleBossEnemyType, GameObject>();
        foreach (var mapping in middleBossMappings)
        {
            if (mapping.prefab != null && !_middleBossDict.ContainsKey(mapping.enemyType))
            {
                _middleBossDict.Add(mapping.enemyType, mapping.prefab);
            }
        }

        _finalBossDict = new Dictionary<Define.FinalBossEnemyType, GameObject>();
        foreach (var mapping in finalBossMappings)
        {
            if (mapping.prefab != null && !_finalBossDict.ContainsKey(mapping.enemyType))
            {
                _finalBossDict.Add(mapping.enemyType, mapping.prefab);
            }
        }
    }

    // Enum을 통해 프리팹을 쉽게 로드하는 외부 호출용 메서드
    public GameObject GetNormalEnemyPrefab(Define.NormalEnemyType type)
    {
        if (_normalEnemyDict == null) InitializeDictionaries();

        if (_normalEnemyDict.TryGetValue(type, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[EnemyPrefabPath] {type}에 해당하는 프리팹이 등록되지 않았습니다.");
        return null;
    }

    public GameObject GetMiddleBossPrefab(Define.MiddleBossEnemyType type)
    {
        if (_middleBossDict == null) InitializeDictionaries();
        return _middleBossDict.TryGetValue(type, out GameObject prefab) ? prefab : null;
    }

    public GameObject GetFinalBossPrefab(Define.FinalBossEnemyType type)
    {
        if (_finalBossDict == null) InitializeDictionaries();
        return _finalBossDict.TryGetValue(type, out GameObject prefab) ? prefab : null;
    }
}