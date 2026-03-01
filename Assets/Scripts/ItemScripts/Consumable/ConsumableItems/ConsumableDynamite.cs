using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[CreateAssetMenu(fileName = "ConsumableWeaponItem_Data", menuName = "Scriptable Object/Consumable Weapon Item Data", order = 1)]
public class ConsumableDynamite : ConsumableItemData
{
    //해당 클래스는 사용하지 않는걸 권장합니다. 대신 ConsumableGrenade를 사용하세요.

    //다이너마이트 아이템 Data
    [SerializeField]
    private GameObject dynamitePrefab;  //다이너마이트 Projectile Prefab

    public override void ActivateItemEffect(PlayerStatus player)
    {
        ThrowBomb(player.transform);
    }

    //다이너마이트 Projectile을 생성하는 메서드
    private void ThrowBomb(Transform playerTf)
    {
        Instantiate(dynamitePrefab, playerTf.position, Quaternion.identity);
        Debug.Log("폭탄 투척");
    }
}
