using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : Status
{
    [SerializeField]
    private EnemyData enemyData;

    public EnemyData EnemyStatusData { get { return enemyData; } }

    void Awake()
    {
        // 스테이터스 데이터 동기화. 추후 scriptableObject 접근으로 변경 예정.
        Hp = enemyData.Hp;
        Damage = enemyData.Damage;
        DamageReduction = enemyData.DamageReduction;
        AttackSpeed = enemyData.AttackSpeed;
        MoveSpeed = enemyData.MoveSpeed;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected override void Dead()
    {
        EnemyAIController enemyAIController = GetComponent<Enemy>().StateMachine;
        enemyAIController.TransitionTo(enemyAIController.deadState);
    }
}
