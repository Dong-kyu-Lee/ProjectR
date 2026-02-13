using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyStatus : Status
{
    [SerializeField]
    private EnemyData enemyData;
    private EnemyLoot enemyLoot;

    [SerializeField]
    private bool isBoss;

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
        AdditionalAttackSpeed = 0;
        AdditionalMoveSpeed = 0;
        enemyLoot = transform.GetComponent<EnemyLoot>();
    }

    void Start()
    {
        
    }

    protected override void HitImpact()
    {
        if (isBoss)
        {
            Vector2 spawnPosition = GetComponent<CapsuleCollider2D>().bounds.min;
            RuneSpawner.Instance.TrySpawnRune(spawnPosition + Vector2.up);
        }
    }

    protected override void Dead()
    {
        if (isDead) return;
        isDead = true;

        EnemyAIController enemyAIController = GetComponent<Enemy>().StateMachine;
        LevelUp.Instance?.IncreaseExp(enemyData.ExpValue);
        enemyLoot.DropLoot();
        Vector2 spawnPosition = GetComponent<CapsuleCollider2D>().bounds.min;
        RuneSpawner.Instance.TrySpawnRune(spawnPosition + Vector2.up);
        enemyAIController.TransitionTo(enemyAIController.deadState);
    }
}
