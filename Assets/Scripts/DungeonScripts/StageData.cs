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
}
