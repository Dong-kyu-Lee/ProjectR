using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Equipment Item Data", order = 1)]
public class EquipmentItemData : BasicItemData
{
    [Header("1. 공격력 (Damage)")]
    [Tooltip("기본 공격력에 더해지는 고정 수치")]
    [SerializeField] private float damage = 0.0f;

    [Header("2. 추가 피해량 (Additional Damage)")]
    [Tooltip("최종 데미지 계산 시 비율(%)로 더해짐")]
    [SerializeField] private float additionalDamage = 0.0f;

    [Header("3. 치명타 확률 (Critical Percent)")]
    [Tooltip("0.1 = 10% 확률")]
    [SerializeField] private float criticalPercent = 0.0f;

    [Header("4. 치명타 피해량 (Critical Damage)")]
    [Tooltip("기본 0.5(50%)에 가산됨. 0.2 입력 시 총 70% 피해")]
    [SerializeField] private float criticalDamage = 0.0f;

    [Header("5. 추가 피해 감소량 (Add. Dmg Reduction)")]
    [Tooltip("피해 감소량과 동일하게 곱연산으로 적용되는 추가 수치")]
    [SerializeField] private float additionalDamageReduction = 0.0f;

    [Header("6. 공격 속도 (Attack Speed)")]
    [Tooltip("기본 공격속도에 비율(%)로 가산됨 (예: 0.2 = 속도 20% 증가)")]
    [SerializeField] private float attackSpeed = 0.0f;

    [Header("7. 이동 속도 (Add. Move Speed)")]
    [Tooltip("이동 속도 고정 수치 증가")]
    [SerializeField] private float additionalmoveSpeed = 0.0f;

    [Header("8. 재화 획득량 (Gold Gain)")]
    [Tooltip("재화 획득 시 가산되는 수치")]
    [SerializeField] private float priceAdditional = 0.0f;

    [Header("9. 피해 감소 무시 (Ignore Def)")]
    [Tooltip("적의 방어력을 무시하는 비율 (예: 0.05 = 5% 무시)")]
    [SerializeField] private float ignoreDamageReduction = 0.0f;

    public void EquipItem(PlayerStatus player)
    {
        if (player == null) return;

        player.Damage += damage;
        player.AdditionalDamage += additionalDamage;
        player.CriticalPercent += criticalPercent;
        player.CriticalDamage += criticalDamage;

        if (additionalDamageReduction != 0f) player.AdditionalDamageReduction += additionalDamageReduction;

        player.AdditionalAttackSpeed += attackSpeed;

        player.AdditionalMoveSpeed += additionalmoveSpeed;

        player.PriceAdditional += priceAdditional;

        player.IgnoreDamageReduction += ignoreDamageReduction;
    }

    public void UnEquipItem(PlayerStatus player)
    {
        if (player == null) return;

        player.Damage -= damage;
        player.AdditionalDamage -= additionalDamage;
        player.CriticalPercent -= criticalPercent;
        player.CriticalDamage -= criticalDamage;

        // 피해 감소 해제 (PlayerStatus Setter가 음수 값을 받으면 Remove 로직 수행)
        if (additionalDamageReduction != 0f) player.AdditionalDamageReduction -= additionalDamageReduction;

        player.AdditionalAttackSpeed -= attackSpeed;
        player.AdditionalMoveSpeed -= additionalmoveSpeed;
        player.PriceAdditional -= priceAdditional;
        player.IgnoreDamageReduction -= ignoreDamageReduction;
    }
}