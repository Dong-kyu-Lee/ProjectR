using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField]
    private GameObject hitBoxObj;

    protected override void Awake()
    {
        base.Awake();
        AttackRangeCol.size = new Vector2(enemyStatus.EnemyStatusData.AttackRange, AttackRangeCol.size.y);
        StateMachine.Initialize(this);
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    protected override void Attack()
    {
        //base.Attack();

        Debug.Log(transform.right.x);
        hitBoxObj.transform.localPosition = new Vector2( (transform.rotation.y != 180f ? -1f : 1f), 0.3f);
        hitBoxObj.SetActive(true);
    }
}
