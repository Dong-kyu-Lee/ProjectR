using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 생성된 방 안에 존재하는 적 오브젝트를 관리하는 클래스
public class EnemyInRoom : MonoBehaviour
{
    private int killCount;

    // 생성할 적 프리펩
    public GameObject firstWaveEnemy;
    public GameObject secondWaveEnemy;

    // 생성된 적 오브젝트
    public List<GameObject> firstWaveEnemyList = new List<GameObject>();
    public List<GameObject> secondWaveEnemyList = new List<GameObject>();

    private void Start()
    {

    }

    // 해당 방에 생성될 모든 적 프리팹을 주어진 위치에 생성하는 함수
    public void SetEnemyTilemap(Room room, Vector3 generatePosition)
    {
        for (int i = 0; i < firstWaveEnemyList.Count; ++i)
            Destroy(firstWaveEnemyList[i]);
        for (int i = 0; i < secondWaveEnemyList.Count; ++i)
            Destroy(secondWaveEnemyList[i]);
        firstWaveEnemyList.Clear();
        secondWaveEnemyList.Clear();

        var enemySpawnPositions = room.enemyTilemap.GetComponentsInChildren<Transform>();
        for(int i = 1; i < enemySpawnPositions.Length; ++i)
        {
            InitEnemySpawner(generatePosition + enemySpawnPositions[i].position);
        }
    }

    // 적 오브젝트를 생성하고 비활성화하는 함수
    private void InitEnemySpawner(Vector3 generatePosition)
    {
        if (firstWaveEnemy != null)
        {
            firstWaveEnemyList.Add(Instantiate(firstWaveEnemy, generatePosition, transform.rotation, transform));
            firstWaveEnemyList[firstWaveEnemyList.Count - 1].GetComponent<Enemy>().onDeath += EnemyDied;
            firstWaveEnemy.SetActive(false);
        }
        if (secondWaveEnemy != null)
        {
            secondWaveEnemyList.Add(Instantiate(secondWaveEnemy, generatePosition, transform.rotation, transform));
            secondWaveEnemyList[secondWaveEnemyList.Count - 1].GetComponent<Enemy>().onDeath += EnemyDied;
            secondWaveEnemy.SetActive(false);
        }
    }

    // 몬스터 웨이브를 생성하는 함수
    public void ActivateEnemies(bool isSecondWave)
    {
        if (!isSecondWave)
        {
            foreach (var item in firstWaveEnemyList)
            {
                item.SetActive(true);
            }
        }
        else
        {
            foreach (var item in secondWaveEnemyList)
            {
                item.SetActive(true);
            }
        }
    }

    int firstIdx, secondIdx;
    private void EnemyDied(Enemy monster)
    {
        if(firstWaveEnemyList.Contains(monster.gameObject))
        {
            firstIdx++;
        }
        else
        {
            secondIdx++;
        }
        monster.onDeath -= EnemyDied;

        // 각 웨이브의 적을 전부 처치했을 때 실행되는 조건문
        if (firstIdx >= firstWaveEnemyList.Count)
        {
            GetComponent<RoomInGame>().onFirstWaveEnd?.Invoke();
            firstIdx = 0;
        }
        if (secondIdx >= secondWaveEnemyList.Count)
        {
            GetComponent<RoomInGame>().onSecondWaveEnd?.Invoke();
            secondIdx = 0;
        }
    }
}
