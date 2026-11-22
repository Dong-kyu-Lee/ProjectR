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

        // 공격 타이밍 설정.
        EnemyAttackTiming timing = new EnemyAttackTiming(0.85f, 0.1f, 0.6f, true, false, false);
        SetAttackStrategy(new MeleeAttackStrategy(timing));
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
        base.PerformAttack();
        StartCoroutine(EnableHitbox());
    }

    IEnumerator EnableHitbox()
    {
        yield return null;
        float hitOffsetX = 0.35f;

        if (hitBoxObj == null) yield break;

        hitBoxObj.transform.localPosition = new Vector2(-hitOffsetX, 0.3f);
        hitBoxObj.SetActive(true);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();
        if (hitBoxObj != null)
        {
            hitBoxObj.SetActive(false);
        }
    }

}
