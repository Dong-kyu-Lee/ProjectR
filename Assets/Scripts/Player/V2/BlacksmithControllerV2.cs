using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithControllerV2 : PlayerControllerBase
{
    private BlacksmithAbilityV2 blacksmithAbility;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;

    protected override void Start()
    {
        base.Start();
        blacksmithAbility = GetComponent<BlacksmithAbilityV2>();
        characterAbility = blacksmithAbility;
    }

    protected override IEnumerator Attack()
    {
        enableAttack = false;
        isAttaking = true;

        // 마우스 방향에 따라 회전
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(playerCamera.transform.position.z);
        Vector3 worldMousePos = playerCamera.ScreenToWorldPoint(mousePos);

        Vector3 direction = worldMousePos - transform.position;
        if (direction.x > 0)
            Flip(1f);
        else
            Flip(-1f);

        playerAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f); // 공격 타이밍과 동기화

        // 적 감지
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float damage = playerStatus.TotalDamage;
                float ignoreReduction = playerStatus.IgnoreDamageReduction;
                bool isCritical = false;

                damage = CalcDamage.Instance.CheckCritical(damage, ref ignoreReduction, ref isCritical);
                hit.GetComponent<Status>()?.TakeDamage(gameObject, damage, ignoreReduction, isCritical);

                CalcDamage.Instance.CheckAddtionalDamage(hit.gameObject);
                CalcDamage.Instance.AdditionalEffect(hit.gameObject);
            }
        }

        yield return new WaitForSeconds(attackCoolTimeA);
        isAttaking = false;

        yield return new WaitForSeconds(1f / playerStatus.TotalAttackSpeed - attackCoolTimeA);
        enableAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
