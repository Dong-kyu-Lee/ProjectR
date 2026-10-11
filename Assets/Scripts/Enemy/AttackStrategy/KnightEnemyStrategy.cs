using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemyStrategy : IAttackStrategy, IComboAttackStrategy
{
    private bool isCloseRangeAttack;
    private bool isAirAttack;
    private int currentComboIndex = 0;

    private float airAttackStartTime;
    private float airAttackWindupDuration;
    private bool hasTeleported = false;
    private bool isAirAttackActive = false;

    // 콤보 공격 타이밍 설정
    private readonly EnemyAttackTiming[] comboTimings = new EnemyAttackTiming[]
    {
        new EnemyAttackTiming(0.42f, 0.16f, 0.16f, false, false, false), // 1타
        new EnemyAttackTiming(0.1f, 0.16f, 0.08f, false, false, false), // 2타
        new EnemyAttackTiming(0.09f, 0.16f, 0.5f, false, false, false), // 3타
        new EnemyAttackTiming(0.17f, 0.16f, 0.84f, false, false, false) // 4타 (마지막 타격은 인터럽트 불가 등)
    };

    // 공중 공격 타이밍 설정
    private readonly EnemyAttackTiming airAttackTiming = new EnemyAttackTiming(0.66f, 0.33f, 0.67f, false, false, false);
    
    // 콤보 인터페이스 구현
    public bool IsComboAttack => isCloseRangeAttack;
    public List<EnemyAttackTiming> GetComboTimings() => new List<EnemyAttackTiming>(comboTimings);
    public void SetCurrentComboIndex(int index) => currentComboIndex = index;

    // 공격 타이밍 반환
    public EnemyAttackTiming Timing
    {
        get
        {
            if (isCloseRangeAttack) return comboTimings[Mathf.Clamp(currentComboIndex, 0, comboTimings.Length - 1)];
            if (isAirAttack) return airAttackTiming;
            return EnemyAttackTiming.Immediate;
        }
    }

    public void BeginAttack(Enemy enemy)
    {
        if (enemy is KnightEnemy knight && knight.PlayerTransform != null)
        {
            // 1. 근접 범위 콤보 공격 실행
            if (knight.IsPlayerInAttackRange)
            {
                isCloseRangeAttack = true;
                isAirAttack = false;
                currentComboIndex = 0;

                if (knight.EnemyAnimator != null)
                    knight.EnemyAnimator.SetTrigger("Attack");
            }
            // 2. 근접 범위 아닌 경우 공중 강하 공격 실행
            else
            {
                isCloseRangeAttack = false;
                isAirAttack = true;
                isAirAttackActive = true;
                hasTeleported = false;

                knight.SetRendererVisible(false);

                knight.transform.position = knight.PlayerTransform.position;

                airAttackStartTime = Time.time;
                airAttackWindupDuration = airAttackTiming.windupTime;

                if (knight.EnemyAnimator != null)
                    knight.EnemyAnimator.SetTrigger("JumpAttack");
            }
        }
    }

    // 공중 강하 공격 진행
    public void UpdateAttacking(Enemy enemy)
    {
        if (isAirAttack && isAirAttackActive && enemy is KnightEnemy knight)
        {
            float elapsed = Time.time - airAttackStartTime;
            float progress = Mathf.Clamp01(elapsed / airAttackWindupDuration);

            // 🟢 2단계: 선딜레이의 30진행 시점에 텔레포트 및 등장 (0.4초 중 약 0.12초 지점)
            if (!hasTeleported && progress >= 0.5f)
            {
                hasTeleported = true;

                // 플레이어 위치로 텔레포트
                if (knight.PlayerTransform != null)
                {
                    knight.transform.position = knight.PlayerTransform.position;
                }

                // 적을 다시 보이게 함 (공중에서 나타남)
                knight.SetRendererVisible(true);
            }

            // 🟢 3단계: 나타난 이후(30% ~ 100%)부터 바닥까지 급강하 진행
            if (hasTeleported)
            {
                float descendProgress = (progress - 0.5f) / 0.5f; // 0.3 ~ 1.0 구간을 0 ~ 1로 매핑
                knight.UpdateAirAttackDescend(descendProgress);
            }
        }
    }

    // 공격 실행
    public void ExecuteAttack(Enemy enemy)
    {
        if (enemy is KnightEnemy knight)
        {
            if (isCloseRangeAttack)
            {
                knight.ActivateHitBoxForCombo(currentComboIndex);
            }
            else if (isAirAttack)
            {
                knight.UpdateAirAttackDescend(1f);
                knight.SetRendererVisible(true);
                knight.ActivateAirAttackHitBox();
            }
        }
    }
}
