using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[CreateAssetMenu(fileName = "ConsumableDynamite_Data", menuName = "Scriptable Object/ConsumableDynamite_Data", order = 1)]
public abstract class EquipmentItemData : BasicItemData
{
    public abstract void EquipItem(PlayerStatus player);  //장착 시 아이템 효과를 발생시키는 메서드
    public abstract void UnEquipItem(PlayerStatus player);    //장착 해제 시 아이템 효과를 해제시키는 메서드
}
