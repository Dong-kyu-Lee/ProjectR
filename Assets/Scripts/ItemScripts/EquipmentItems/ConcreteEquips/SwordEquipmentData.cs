using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordEquipment", menuName = "Scriptable Object/Sword Equipments Item Data", order = 1)]
public class SwordEquipmentData : EquipmentItemData
{
    [SerializeField]
    private float additionalDamage = 10.0f;

    public override void EquipItem(PlayerStatus playerStatus)
    {
        playerStatus.AdditionalDamage += additionalDamage;
        //Debug.Log($"아이템 장착함 : {playerStatus.AdditionalDamage}");

    }

    public override void UnEquipItem(PlayerStatus playerStatus)
    {
        playerStatus.AdditionalDamage -= additionalDamage;
        //Debug.Log($"아이템 장착 해제함 : {playerStatus.AdditionalDamage}");
    }
}
