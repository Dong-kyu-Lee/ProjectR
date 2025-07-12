using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyPrefab1;
    [SerializeField]
    private List<GameObject> enemyPrefab2;

    public GameObject GetRandomEnemyPrefab1()
    {
        if (enemyPrefab1.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs available in enemyPrefab1 list.");
            return null;
        }
        int randomIndex = Random.Range(0, enemyPrefab1.Count);
        return enemyPrefab1[randomIndex];
    }

    public GameObject GetRandomEnemyPrefab2()
    {
        if (enemyPrefab2.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs available in enemyPrefab2 list.");
            return null;
        }
        int randomIndex = Random.Range(0, enemyPrefab2.Count);
        return enemyPrefab2[randomIndex];
    }
}
