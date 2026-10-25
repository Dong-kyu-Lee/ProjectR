using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithControllerV2 : PlayerControllerBase
{
    private BlacksmithAbilityV2 blacksmithAbility;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField, Min(0.1f)] private float attackHeight = 1.2f;

    private static readonly int EnchantState = Animator.StringToHash("Base Layer.Enchant");

    [SerializeField] private float attackStepForce = 7f;     // 공격 시 앞으로 전진하는 속도
    [SerializeField] private float attackDeceleration = 15f; // 전진 후 멈추는 감속도 (높을수록 빨리 멈춤)

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    [SerializeField] private float comboLimitTime = 1.5f; // 콤보를 이어가기 위한 유효 시간

    protected override void Start()
    {
        base.Start();
        blacksmithAbility = GetComponent<BlacksmithAbilityV2>();
        characterAbility = blacksmithAbility;
        blacksmithAbility.onEnchantPerformed.AddListener(PlayEnchantAnimation);
    }

    protected override void Update()
    {
        base.Update();

        // 현재 플레이어의 공격 속도를 애니메이터 배속 파라미터에 전달
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("AnimAttackSpeed", playerStatus.TotalAttackSpeed);

            // 점프/낙하 애니메이션을 위해 현재 Y축 속도를 애니메이터로 전달
            playerAnimator.SetFloat("yVelocity", playerRigidBody.linearVelocity.y);
        }
    }

    protected override void FixedUpdate()
    {
        if (isPause || isDead) return;

        JumpCheck();

        // 대장장이는 공격 중일 때 일반 키보드 이동을 하지 않음
        if (isAttaking && isGround && !isDashing)
        {
            // 공격 코루틴에서 부여한 x축 오프셋 속도를 부드럽게 감속시켜
            // 얼음판처럼 미끄러지지 않고 묵직하게 정지하도록 처리
            Vector2 vel = playerRigidBody.linearVelocity;
            vel.x = Mathf.Lerp(vel.x, 0, Time.fixedDeltaTime * attackDeceleration);
            playerRigidBody.linearVelocity = vel;
        }
        else
        {
            // 공격 중이 아닐 때만 부모 클래스의 일반 이동 실행
            base.FixedUpdate();
        }
    }

    private void LateUpdate()
    {
        if (isPause || isDead || !isAttaking) return;

        // Animator가 이번 프레임의 상태와 전환을 평가한 뒤 이동 제한을 해제한다.
        // 공격 입력 간격과 실제 모션 길이는 서로 다르므로 별도로 처리한다.
        if (!IsAttackAnimationPlaying())
        {
            isAttaking = false;
            ResetCombo();
        }
    }

    private bool IsAttackAnimationPlaying()
    {
        if (playerAnimator == null || !playerAnimator.isActiveAndEnabled
            || playerAnimator.runtimeAnimatorController == null) return false;

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return true;

        return playerAnimator.IsInTransition(0)
            && playerAnimator.GetNextAnimatorStateInfo(0).IsTag("Attack");
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

        if (isGround)
        {
            playerRigidBody.linearVelocity = new Vector2(sx * attackStepForce, playerRigidBody.linearVelocity.y);
        }

        // 사운드
        if (blacksmithAbility.CurWeaponData.WeaponStyle == WeaponStyle.OneHanded) SoundManager.Instance.Play("Sounds/battle/swing", Sound.AttackEffect, 0.4f);
        else if (blacksmithAbility.CurWeaponData.WeaponStyle == WeaponStyle.TwoHanded) SoundManager.Instance.Play("Sounds/battle/swing3", Sound.AttackEffect, 0.4f);

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
            playerAnimator.ResetTrigger("Attack");
            playerAnimator.SetInteger("ComboStep", comboStep);
            playerAnimator.SetTrigger("Attack");
        }

        if (attackPoint == null)
        {
            goto Cooldown;
        }

        // 가로 사거리와 세로 높이를 분리해 다른 층의 적까지 맞지 않도록 한다.
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, GetAttackSize(), 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                // 등 뒤에 있는 적 판정 무시하기
                // 적과 나의 X 좌표 차이 계산 (적이 내 오른쪽에 있으면 양수, 왼쪽에 있으면 음수)
                float distanceX = hit.transform.position.x - transform.position.x;

                // 두 값을 곱해서 음수(-)가 나오면 적이 내 공격 방향 반대(등 뒤)에 있다는 뜻
                if (distanceX * sx < -0.2f)
                {
                    continue; // 데미지를 주지 않고 다음 적 검사로 넘어감
                }

                float damage = playerStatus.TotalDamage;
                float ignoreReduction = playerStatus.IgnoreDamageReduction;
                bool isCritical = false;

                damage = CalcDamage.Instance.CheckCritical(damage, ref ignoreReduction, ref isCritical);
                hit.GetComponent<Status>()?.TakeDamage(gameObject, damage, ignoreReduction, isCritical);

                if (AbilityManager.Instance.blacksmithAbility[3]) playerStatus.Hp += damage * 0.02f;

                CalcDamage.Instance.CheckAddtionalDamage(hit.gameObject);
                CalcDamage.Instance.AdditionalEffect(hit.gameObject);
                CalcDamage.Instance.CheckFightState();

                // 타격 성공 시 적에게 무기 데이터에 따른 경직 부여
                Enemy hitEnemy = hit.GetComponent<Enemy>();
                if (hitEnemy != null)
                {
                    float currentStunTime = 0.3f; // 기본값

                    // 현재 장착된 무기 데이터가 있다면 그 무기의 경직 시간을 가져옴
                    if (blacksmithAbility != null && blacksmithAbility.CurWeaponData != null)
                    {
                        currentStunTime = blacksmithAbility.CurWeaponData.HitStunDuration;
                    }

                    hitEnemy.ApplyHitStun(currentStunTime);
                }
            }
        }

    Cooldown:
        float fullActionTime = attackCoolTimeA / playerStatus.TotalAttackSpeed;

        // 공격 간격의 70% 지점부터 재입력 허용 (실제 모션 종료와 별도)
        float cancelTime = fullActionTime * 0.7f;
        yield return new WaitForSeconds(cancelTime);

        // 이 시점부터 다음 콤보 입력을 미리 허용
        enableAttack = true;

        yield return new WaitForSeconds(fullActionTime - cancelTime);

        if (Time.time - lastAttackTime >= fullActionTime * 0.9f)
        {
            // 콤보만 정리하고, 이동 제한 해제는 LateUpdate에서 모션 종료에 맞춘다.
            ResetCombo();
        }
    }

    private void ResetCombo()
    {
        comboStep = 0;
        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("ComboStep", 0);
        }
    }

    private Vector2 GetAttackSize()
    {
        return new Vector2(attackRange * 2f, attackHeight);
    }

    private void PlayEnchantAnimation()
    {
        if (isDead || isPause || isDashing || !isGround || playerAnimator == null) return;
        if (!playerAnimator.HasState(0, EnchantState)) return;

        // 강화는 타격 판정이나 전진 없이 재생하며, 다음 행동으로 중단할 수 있다.
        isAttaking = false;
        ResetCombo();
        playerAnimator.ResetTrigger("Attack");
        playerAnimator.Play(EnchantState, 0, 0f);
    }

    private void OnDestroy()
    {
        if (blacksmithAbility != null)
            blacksmithAbility.onEnchantPerformed.RemoveListener(PlayEnchantAnimation);
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
            Gizmos.DrawWireCube(attackPoint.position, GetAttackSize());
        }
    }
}
