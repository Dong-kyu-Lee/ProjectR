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

        var cam = GetActiveCamera();
        if (cam == null)
        {
            goto Cooldown;
        }
        var sr = GetSprite();
        if (sr == null)
        {
            goto Cooldown;
        }

        float px = cam.WorldToScreenPoint(transform.position).x;
        float dx = Input.mousePosition.x - px;
        float sx = (Mathf.Abs(dx) < 1f) ? (sr.flipX ? 1f : -1f) : Mathf.Sign(dx);

        bool aimRight = sx > 0f;
        if (aimRight != sr.flipX) Flip(sx);

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Attack");

        if (attackPoint == null)
        {
            goto Cooldown;
        }

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

    Cooldown:
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
