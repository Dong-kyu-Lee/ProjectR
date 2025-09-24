using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BartenderControllerV2: PlayerControllerBase 상속 및 공격 방식 구현
public class BartenderControllerV2 : PlayerControllerBase
{
    public GameObject projectilePrefab;
    private BartenderAbilityV2 bartenderAbility;

    protected override void Start()
    {
        base.Start();
        bartenderAbility = GetComponent<BartenderAbilityV2>();
        characterAbility = bartenderAbility;
    }

    protected override IEnumerator Attack()
    {
        enableAttack = false;
        isAttaking = true;

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Attack");

        // 카메라/스프라이트 안전 취득
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

        // 클릭 좌표 기준 방향 (화면 공간)
        Vector2 playerScreen = cam.WorldToScreenPoint(transform.position);
        Vector2 sdir = (Vector2)Input.mousePosition - playerScreen;
        if (sdir.sqrMagnitude < 1f)
            sdir = sr.flipX ? Vector2.right : Vector2.left; // 너무 가까이 클릭하면 보정

        Vector2 direction = sdir.normalized;

        // 클릭 방향과 반대면 플립
        bool aimRight = direction.x > 0f;
        if (aimRight != sr.flipX) Flip(direction.x);

        // 프리팹/능력 null 가드
        if (projectilePrefab == null)
        {
            goto Cooldown;
        }
        if (bartenderAbility == null)
            bartenderAbility = GetComponent<BartenderAbilityV2>();

        // 발사
        Vector3 spawnPos = transform.position + (Vector3)direction * projectileSpawnOffset;
        sr.flipX = spawnPos.x > transform.position.x;

        var projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        var projComp = projectile.GetComponent<ProjectileV2>();
        if (projComp != null)
        {
            projComp.Velocity = direction;
            projComp.playerStatus = playerStatus;
            projComp.player = gameObject;
            if (bartenderAbility != null)
            {
                projComp.bartenderAbility = bartenderAbility;
                projComp.bottle = bartenderAbility.UseBartenderBottle();
            }
        }

        // 2발 옵션 — 매니저/배열 null 가드
        if (AbilityManager.Instance != null &&
            AbilityManager.Instance.bartenderAbility != null &&
            AbilityManager.Instance.bartenderAbility.Length > 0 &&
            AbilityManager.Instance.bartenderAbility[0])
        {
            var projectile2 = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            var proj2Comp = projectile2.GetComponent<ProjectileV2>();
            if (proj2Comp != null)
            {
                Vector2 offsetDir = (direction + Vector2.up * 0.2f).normalized;
                proj2Comp.Velocity = offsetDir;
                proj2Comp.playerStatus = playerStatus;
                proj2Comp.player = gameObject;
                if (bartenderAbility != null)
                {
                    proj2Comp.bartenderAbility = bartenderAbility;
                    proj2Comp.bottle = bartenderAbility.UseBartenderBottle();
                }
            }
        }

    Cooldown:
        yield return new WaitForSeconds(attackCoolTimeA);
        isAttaking = false;
        yield return new WaitForSeconds(1f / playerStatus.TotalAttackSpeed - attackCoolTimeA);
        enableAttack = true;
    }

}
