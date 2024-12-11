using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public List<GameObject> enemyKind; // 생성할 적 종류
    [SerializeField]
    private List<GameObject> firstWaveEnemies; // 맵 위에 생성된 첫번째 웨이브 적 오브젝트
    [SerializeField]
    private List<GameObject> secondWaveEnemies; // 두번째 웨이브 적 오브젝트
    [SerializeField]
    private int enemyCountOnWave = 0; // 웨이브에 생성된 적 숫자를 카운트
    private int deadEnemyCount = 0; // 죽은 적 숫자 카운트

    private void Start()
    {
        DungeonFlowManager.Instance.EnemySpawnManager = this;
    }

    // 적을 생성하는 함수
    public void GenerateEnemies()
    {
        RemoveAllEnemies();

        List<Room> generatedRooms = new List<Room>();

        List<GameObject> roomObjects = gameObject.GetComponent<DungeonCreator>().generatedRooms;
        foreach (GameObject obj in roomObjects)
        {
            generatedRooms.Add(obj.GetComponent<Room>());
        }

        foreach (Room room in generatedRooms)
        {
            if (room.enemySpawnPoint.Count <= 0) continue;

            foreach(Transform enemyPos in room.enemySpawnPoint)
            {
                firstWaveEnemies.Add(Instantiate(enemyKind[Random.Range(0, enemyKind.Count)],
                    enemyPos.position, transform.rotation));
                secondWaveEnemies.Add(Instantiate(enemyKind[Random.Range(0, enemyKind.Count)],
                    enemyPos.position, transform.rotation));
                enemyCountOnWave++;
            }
        }

        // 두번째 웨이브 적들은 비활성화
        for(int i = 0; i < secondWaveEnemies.Count; ++i)
        {
            secondWaveEnemies[i].SetActive(false);
        }
    }

    // 적 사망 시 적 클래스에서 호출하는 함수. 
    public void EnemyDeadCount()
    {
        deadEnemyCount++;

        if(deadEnemyCount == enemyCountOnWave) // 첫번째 웨이브 클리어
        {
            for (int i = 0; i < secondWaveEnemies.Count; ++i)
            {
                secondWaveEnemies[i].SetActive(true);
            }
        }
        if(deadEnemyCount == enemyCountOnWave * 2) // 두번째 웨이브 클리어
        {
            // Finish Spot 활성화
            DungeonFlowManager.Instance.OpenFinishSpot();
            Debug.Log("Finish Spot 열린 상태");
            deadEnemyCount = 0;
            enemyCountOnWave = 0;
        }
    }

    public void RemoveAllEnemies()
    {
        for (int i = 0; i < firstWaveEnemies.Count; ++i)
        {
            if(firstWaveEnemies[i] != null)
                Destroy(firstWaveEnemies[i]);
        }
        firstWaveEnemies.Clear();

        for (int i = 0; i < secondWaveEnemies.Count; ++i)
        {
            if (secondWaveEnemies[i] != null)
                Destroy(secondWaveEnemies[i]);
        }
        secondWaveEnemies.Clear();
    }

    #region 에디터 테스트용
    int tmp = 0;
    public void KillFirstWaveEnemy()
    {
        if(tmp >= firstWaveEnemies.Count)
        {
            Debug.Log("First Wave Enemies are all dead");
            return;
        }
        firstWaveEnemies[tmp].SetActive(false);
        EnemyDeadCount();
        tmp++;
    }
    int tmp2 = 0;
    public void KillSecondWaveEnemy()
    {
        if (tmp2 >= secondWaveEnemies.Count)
        {
            Debug.Log("First Wave Enemies are all dead");
            return;
        }
        secondWaveEnemies[tmp2].SetActive(false);
        EnemyDeadCount();
        tmp2++;
    }
    #endregion
}
