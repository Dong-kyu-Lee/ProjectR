using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Define.Enemy
{
    public enum NormalEnemyType
    {
        MeleeEnemy, RangedEnemy, MeleeDebuffEnemy
    }
    public enum MiddleBossEnemyType
    {
        Queen, 
    }
    public enum FinalBossEnemyType
    {
        Hero, 
    }
    
    // 스포너의 인스펙터 창에서 적 종류만 선택하기 위한 데이터 클래스
    [System.Serializable]
    public class NormalEnemyData
    {
        public NormalEnemyType enemyType;
    }

    // ScriptableObject에서 Enum과 프리팹을 1:1 매핑하기 위한 직렬화 구조체
    [System.Serializable]
    public struct NormalEnemyPrefabMapping
    {
        public NormalEnemyType enemyType;
        public GameObject prefab;
    }

    [System.Serializable]
    public struct MiddleBossPrefabMapping
    {
        public MiddleBossEnemyType enemyType;
        public GameObject prefab;
    }

    [System.Serializable]
    public struct FinalBossPrefabMapping
    {
        public FinalBossEnemyType enemyType;
        public GameObject prefab;
    }
}
