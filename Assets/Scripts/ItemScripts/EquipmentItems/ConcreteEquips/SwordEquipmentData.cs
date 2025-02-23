using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SwordEquipment", menuName = "Scriptable Object/Sword Equipments Item Data", order = 1)]
public class SwordEquipmentData : EquipmentItemData
{
    [SerializeField]
    private float additionalAttackDamage = 10.0f;   //추가 공격력
    [SerializeField]
    private float additionalAttackSpeed = 0.0f;     //추가 공격속도

    public override void EquipItem(PlayerStatus playerStatus)
    {
        playerStatus.AdditionalDamage += additionalAttackDamage;
        playerStatus.AdditionalAttackSpeed += additionalAttackSpeed;
    }

    public override void UnEquipItem(PlayerStatus playerStatus)
    {
        playerStatus.AdditionalDamage -= additionalAttackDamage;
        playerStatus.AttackSpeed -= additionalAttackSpeed;
    }
}
