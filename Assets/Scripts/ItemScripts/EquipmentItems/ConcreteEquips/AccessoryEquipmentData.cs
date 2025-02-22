using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AccessoryEquipment", menuName = "Scriptable Object/Accessory Equipment Item Data", order = 1)]
public class AccessoryEquipmentData : EquipmentItemData
{
    [SerializeField]
    private float additionalAttackSpeed = 0.0f;
    [SerializeField]
    private float additionalMoveSpeed = 0.0f;
    [SerializeField]
    private float additionalPriceAdd = 0.0f;
    [SerializeField]
    private float additionalDamage = 0.0f;
    [SerializeField]
    private float additionalDamageReduction = 0.0f;


    public override void EquipItem(PlayerStatus player)
    {
        player.AdditionalAttackSpeed += additionalAttackSpeed;
        player.MoveSpeed += additionalMoveSpeed;
        player.PriceAdditional += additionalPriceAdd;
        player.AdditionalDamage += additionalDamage;
        player.AdditionalDamageReduction += additionalDamageReduction;
    }

    public override void UnEquipItem(PlayerStatus player)
    {
        player.AdditionalAttackSpeed -= additionalAttackSpeed;
        player.MoveSpeed -= additionalMoveSpeed;
        player.PriceAdditional -= additionalPriceAdd;
        player.AdditionalDamage -= additionalDamage;
        player.AdditionalDamageReduction -= additionalDamageReduction;
    }
}
