using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{   
    private void Awake()
    {
        AttackRangeCol.size = new Vector2(enemyStatus.EnemyStatusData.AttackRange, AttackRangeCol.size.y);
        StateMachine.Initialize(this);
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
