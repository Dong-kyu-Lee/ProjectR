using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField]
    public GameObject hitBoxObj;

    protected override void Awake()
    {
        base.Awake();
        SetAttackStrategy(new MeleeAttackStrategy());
        AttackRangeCol.size = new Vector2(enemyStatus.EnemyStatusData.AttackRange, AttackRangeCol.size.y);
    }

    void Start()
    {
        StateMachine.Initialize(this);
    }

    void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    public override void PerformAttack()
    {
        float hitOffsetX = 0.6f;

        hitBoxObj.transform.localPosition = new Vector2(-hitOffsetX, 0.3f);
        hitBoxObj.SetActive(true);
    }

}
