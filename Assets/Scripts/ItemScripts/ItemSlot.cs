using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    private ConsumableItemData itemData = null; //현재 슬롯에 등록된 아이템 정보
    private int currentAmount = 0;              //현재 슬롯에 등록된 아이템의 갯수

    public ConsumableItemData ItemData { get { return itemData; } }
    public int CurrentAmount { 
        get { return currentAmount; }
        set 
        { 
            if(value > 0) currentAmount = value;
            else
            {
                currentAmount = 0;
                itemData = null;
            }
        }
    }

    //슬롯에 아이템을 등록하고 갯수를 Amount개로 설정하는 메서드
    public void PutItem(ConsumableItemData item, int Amount = 1)
    {
        itemData = item;
        currentAmount += Amount;
    }

    //해당 슬롯으로 등록된 아이템을 사용하는 메서드
    public void UseItem(GameObject player)
    {
        if(itemData == null)
        {
            Debug.Log("현재 등록된 소모품이 없습니다.");
            return;
        }

        if (currentAmount > 0)
        {
            itemData.ActivateItemEffect(player);
            currentAmount--;
        }
        else
        {
            Debug.Log($"아이템의 갯수가 부족합니다 : {itemData.ItemName}");
        }
    }
}
