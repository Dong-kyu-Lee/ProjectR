using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject firstWaveEnemy;
    public GameObject secondWaveEnemy;

    void Start()
    {
        if (firstWaveEnemy != null)
        {
            firstWaveEnemy = Instantiate(firstWaveEnemy, transform.position, transform.rotation);
            firstWaveEnemy.SetActive(false);
        }
        if (secondWaveEnemy != null)
        {
            secondWaveEnemy = Instantiate(secondWaveEnemy, transform.position, transform.rotation);
            secondWaveEnemy.SetActive(false);
        }
    }

    public void SpawnEnemy(bool isSecondWave)
    {
        if (!isSecondWave) firstWaveEnemy.SetActive(true);
        else secondWaveEnemy.SetActive(true);
    }
}
