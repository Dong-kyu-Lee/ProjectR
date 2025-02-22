using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ArmorEquipment", menuName = "Scriptable Object/Armor Equipment Item Data", order = 1)]
public class ArmorEquipmentData : EquipmentItemData
{
    [SerializeField]
    private float additionalDmgReduction = 0.0f;    //추가 피해무시량
    [SerializeField]
    private float additionalMoveSpeed = 0.0f;       //추가 이동속도량


    public override void EquipItem(PlayerStatus player)
    {
        player.AdditionalDamageReduction += additionalDmgReduction;
        player.MoveSpeed += additionalMoveSpeed;
    }

    public override void UnEquipItem(PlayerStatus player)
    {
        player.AdditionalDamageReduction -= additionalDmgReduction;
        player.MoveSpeed -= additionalMoveSpeed;
    }
}