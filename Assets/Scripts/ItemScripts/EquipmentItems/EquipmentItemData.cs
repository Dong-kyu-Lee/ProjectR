using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Equipment Item Data", order = 1)]
public class EquipmentItemData : ScriptableObject
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

    [Header("5. 피해 감소량 (Damage Reduction)")]
    [Tooltip("곱연산으로 적용되는 피해 감소율 (예: 0.1 = 10% 감소)")]
    [SerializeField] private float damageReduction = 0.0f;

    [Header("6. 추가 피해 감소량 (Add. Dmg Reduction)")]
    [Tooltip("피해 감소량과 동일하게 곱연산으로 적용되는 추가 수치")]
    [SerializeField] private float additionalDamageReduction = 0.0f;

    [Header("7. 공격 속도 (Attack Speed)")]
    [Tooltip("기본 공격속도에 비율(%)로 가산됨 (예: 0.2 = 속도 20% 증가)")]
    [SerializeField] private float attackSpeed = 0.0f;

    [Header("8. 이동 속도 (Move Speed)")]
    [Tooltip("이동 속도 고정 수치 증가")]
    [SerializeField] private float moveSpeed = 0.0f;

    [Header("9. 재화 획득량 (Gold Gain)")]
    [Tooltip("재화 획득 시 가산되는 수치")]
    [SerializeField] private float priceAdditional = 0.0f;

    public void EquipItem(PlayerStatus player)
    {
        if (player == null) return;

        player.Damage += damage;
        player.AdditionalDamage += additionalDamage;
        player.CriticalPercent += criticalPercent;
        player.CriticalDamage += criticalDamage;

        if (damageReduction != 0f) player.AdditionalDamageReduction += damageReduction;
        if (additionalDamageReduction != 0f) player.AdditionalDamageReduction += additionalDamageReduction;

        player.AdditionalAttackSpeed += attackSpeed;

        player.MoveSpeed += moveSpeed;

        player.PriceAdditional += priceAdditional;
    }

    public void UnEquipItem(PlayerStatus player)
    {
        if (player == null) return;

        player.Damage -= damage;
        player.AdditionalDamage -= additionalDamage;
        player.CriticalPercent -= criticalPercent;
        player.CriticalDamage -= criticalDamage;

        // 피해 감소 해제 (PlayerStatus Setter가 음수 값을 받으면 Remove 로직 수행)
        if (damageReduction != 0f) player.AdditionalDamageReduction -= damageReduction;
        if (additionalDamageReduction != 0f) player.AdditionalDamageReduction -= additionalDamageReduction;

        player.AdditionalAttackSpeed -= attackSpeed;
        player.MoveSpeed -= moveSpeed;
        player.PriceAdditional -= priceAdditional;
    }
}