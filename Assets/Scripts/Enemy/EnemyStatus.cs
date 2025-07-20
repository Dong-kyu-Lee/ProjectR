using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyStatus : Status
{
    [SerializeField]
    private EnemyData enemyData;

    public EnemyData EnemyStatusData { get { return enemyData; } }

    void Awake()
    {
        // 스테이터스 데이터 동기화. 추후 scriptableObject 접근으로 변경 예정.
        MaxHp = enemyData.Hp;
        Hp = MaxHp;
        Damage = enemyData.Damage;
        DamageReduction = enemyData.DamageReduction;
        AttackSpeed = enemyData.AttackSpeed;
        MoveSpeed = enemyData.MoveSpeed;
        AdditionalMoveSpeed = 0;
    }

    void Start()
    {
        
    }

    protected override void Dead()
    {
        EnemyAIController enemyAIController = GetComponent<Enemy>().StateMachine;
        LevelUp.Instance.IncreaseExp(enemyData.ExpValue);
        Vector2 spawnPosition = GetComponent<CapsuleCollider2D>().bounds.min;
        RuneSpawner.Instance.TrySpawnRune(spawnPosition + Vector2.up);
        enemyAIController.TransitionTo(enemyAIController.deadState);
    }
}
