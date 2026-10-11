using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttackScanner : MonoBehaviour
{
    private Enemy enemy;
    private KnightEnemy knightEnemy;

    [Header("Air Attack Settings")]
    [SerializeField] private float airAttackCooldown = 10.0f;
    private float lastAirAttackTime = -999f;

    // 쿨타임 체크 프로퍼티
    public bool CanUseAirAttack => Time.time >= lastAirAttackTime + airAttackCooldown;

    // 공중 공격 실행 시 쿨타임 갱신
    public void UseAirAttack()
    {
        lastAirAttackTime = Time.time;
    }

    private void Awake()
    {
        enemy = transform.parent.GetComponent<Enemy>();
        knightEnemy = transform.parent.GetComponent<KnightEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TryStartAirAttack();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TryStartAirAttack();
        }
    }

    private void TryStartAirAttack()
    {
        var sm = enemy.StateMachine;

        // 이미 죽었거나 공격 중이면 새로 시작하지 않음
        if (sm.isDead) return;
        if (sm.CurrentState == sm.attackState) return;

        // 쿨타임이 찼을 때만 공중 공격 상태로 진입
        if (CanUseAirAttack)
        {
            UseAirAttack();
            enemy.StartAttack();
        }
        // 쿨타임 중이면 아무것도 하지 않아 ChaseState를 유지함
    }
}
