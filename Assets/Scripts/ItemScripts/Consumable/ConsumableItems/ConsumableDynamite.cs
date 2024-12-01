using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableDynamite_Data", menuName = "Scriptable Object/ConsumableDynamite_Data", order = 1)]
public class ConsumableDynamite : ConsumableItemData
{
    //다이너마이트 아이템 Data
    [SerializeField]
    private GameObject dynamitePrefab;  //다이너마이트 Projectile Prefab

    public override void ActivateItemEffect(GameObject player)
    {
        ThrowBomb(player);
    }

    //다이너마이트 Projectile을 생성하는 메서드
    private void ThrowBomb(GameObject player)
    {
        Instantiate(dynamitePrefab, GameObject.Find("Player").transform.position, Quaternion.identity);
        Debug.Log("폭탄 투척");
    }
}
