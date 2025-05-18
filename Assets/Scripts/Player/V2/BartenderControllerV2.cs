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
        playerAnimator.SetTrigger("Attack");

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(playerCamera.transform.position.z);
        Vector3 curMousePoint = playerCamera.ScreenToWorldPoint(mousePos);

        Vector3 direction = curMousePoint - transform.position;
        if (direction.magnitude < 0.5f)
        {
            direction = (direction.x > 0 ? Vector3.right : Vector3.left);
        }
        else
        {
            direction.Normalize();
        }

        Vector3 spawnPos = transform.position + direction * projectileSpawnOffset;
        rendererObject.GetComponent<SpriteRenderer>().flipX = spawnPos.x > transform.position.x;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        var projComp = projectile.GetComponent<ProjectileV2>();
        projComp.Velocity = direction;
        projComp.playerStatus = playerStatus;
        projComp.player = gameObject;
        projComp.bartenderAbility = bartenderAbility;
        projComp.bottle = bartenderAbility.UseBartenderBottle();

        yield return new WaitForSeconds(attackCoolTimeA);
        isAttaking = false;
        yield return new WaitForSeconds(1f / playerStatus.TotalAttackSpeed - attackCoolTimeA);
        enableAttack = true;
    }
}
