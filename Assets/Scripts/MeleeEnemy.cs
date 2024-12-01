using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField]
    private BoxCollider2D attackRangeCol;

    [SerializeField]
    private EnemyAIController enemyController;

    public EnemyAIController StateMachine { get; }
    
    private void Awake()
    {
        attackRangeCol.size = new Vector2(enemyStatus.EnemyStatusData.AttackRange, attackRangeCol.size.y);
        enemyController.Initialize(this);
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.name == "Player")
        {
            enemyAnimator.SetTrigger("Attack");
        }
    }
}
