using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithControllerV2 : PlayerControllerBase
{
    private BlacksmithAbilityV2 blacksmithAbility;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    [SerializeField] private float comboLimitTime = 1.5f; // 콤보를 이어가기 위한 유효 시간

    protected override void Start()
    {
        base.Start();
        blacksmithAbility = GetComponent<BlacksmithAbilityV2>();
        characterAbility = blacksmithAbility;
    }

    protected override void Update()
    {
        base.Update();

        // 현재 공격 속도에 비례한 '최소 공격 모션 시간'
        float currentActionTime = attackCoolTimeA / playerStatus.TotalAttackSpeed;

        // 이 시간 안에 다음 클릭을 해야 콤보가 유지됨 (모션 시간 + 여유 시간)
        float dynamicComboLimit = currentActionTime + 0.35f;

        if (comboStep > 0 && Time.time - lastAttackTime > dynamicComboLimit)
        {
            ResetCombo();
        }
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

        // 무기 스타일에 따른 최대 콤보 수 결정
        int maxCombo = 2; // 기본값
        if (blacksmithAbility != null && blacksmithAbility.CurWeaponData != null)
        {
            maxCombo = blacksmithAbility.CurWeaponData.WeaponStyle == WeaponStyle.OneHanded ? 2 : 4;
        }

        // 콤보 인덱스 증가 및 순환
        comboStep++;
        if (comboStep > maxCombo) comboStep = 1;
        lastAttackTime = Time.time;

        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("ComboStep", comboStep);
            playerAnimator.SetTrigger("Attack");
        }

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
        float currentActionTime = attackCoolTimeA / playerStatus.TotalAttackSpeed;
        yield return new WaitForSeconds(currentActionTime);

        isAttaking = false;
        enableAttack = true;
    }

    private void ResetCombo()
    {
        comboStep = 0;
        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("ComboStep", 0);
        }
    }

    public override void ResetPlayerState()
    {
        base.ResetPlayerState();
        ResetCombo();
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
