using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    [SerializeField]
    private ConsumableItemData myItemData;  //필드에 떨어진 아이템의 정보
    [SerializeField]
    private int amount = 1;                 //습득했을 경우 얻게되는 아이템 수

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = myItemData.itemSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AddToPlayerInventory(collision.GetComponent<PlayerItemUseController>());
        }
    }

    //자신의 아이템 정보 및 수량을 플레이어의 인벤토리에 넣는 메서드
    private void AddToPlayerInventory(PlayerItemUseController itemUseController)
    {
        if(itemUseController.AddItem(myItemData, amount))
        {
            myItemData = null;
            Destroy(gameObject);
        }
    }
}
