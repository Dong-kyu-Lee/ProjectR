using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대장장이 캐릭터 컨트롤러 V2 (공통 컨트롤러 기반)
/// </summary>
public class BlacksmithControllerV2 : PlayerControllerBase
{
    private BlacksmithAbilityV2 blacksmithAbility;

    private float minDistance = 0.5f;

    protected override void Start()
    {
        base.Start();
        blacksmithAbility = GetComponent<BlacksmithAbilityV2>();
        characterAbility = blacksmithAbility;
    }

    // 대장장이 전용 공격: 추후 구현 예정
    protected override IEnumerator Attack()
    {
        enableAttack = false;
        isAttaking = true;
        playerAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCoolTimeA);
        isAttaking = false;

        yield return new WaitForSeconds(1f / playerStatus.TotalAttackSpeed - attackCoolTimeA);
        enableAttack = true;
    }
}
