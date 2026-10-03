using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyPrefabPath enemyPrefabPath;

    [Header("Enemy Spawn Lists")]
    [SerializeField] private List<Define.NormalEnemyType> enemyList1; 
    [SerializeField] private List<Define.NormalEnemyType> enemyList2; 

    public GameObject GetRandomEnemyPrefab1()
    {
        if (enemyPrefabPath == null)
        {
            Debug.LogError("enemyPrefabPath가 할당되지 않았습니다.", this);
            return null;
        }
        if (enemyList1.Count == 0)
        {
            Debug.LogWarning("enemyList1이 비어 있습니다.");
            return null;
        }

        // 1. 랜덤하게 적 종류(Data)를 선택
        int randomIndex = Random.Range(0, enemyList1.Count);
        Define.NormalEnemyType selectedType = enemyList1[randomIndex];

        // 2. ScriptableObject를 통해 Enum에 대응하는 실제 프리팹을 가져옴
        return enemyPrefabPath.GetNormalEnemyPrefab(selectedType);
    }

    public GameObject GetRandomEnemyPrefab2()
    {
        if (enemyPrefabPath == null)
        {
            Debug.LogError("enemyPrefabPath가 할당되지 않았습니다.", this);
            return null;
        }
        if (enemyList2.Count == 0)
        {
            Debug.LogWarning("enemyList2가 비어 있습니다.");
            return null;
        }

        int randomIndex = Random.Range(0, enemyList2.Count);
        Define.NormalEnemyType selectedType = enemyList2[randomIndex];

        return enemyPrefabPath.GetNormalEnemyPrefab(selectedType);
    }
}
