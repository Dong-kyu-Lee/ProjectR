using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScanner : MonoBehaviour
{
    private Enemy enemy;
    private bool playerInRange;

    private void Awake()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            TryStartAttack();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            TryStartAttack();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void TryStartAttack()
    {
        var sm = enemy.StateMachine;

        // 이미 죽었거나 공격 중이면 새로 시작하지 않음
        if (sm.isDead) return;
        if (sm.CurrentState == sm.attackState) return;

        enemy.StartAttack();
    }
}

