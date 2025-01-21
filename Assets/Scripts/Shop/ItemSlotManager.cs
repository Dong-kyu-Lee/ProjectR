using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotManager : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] itemImage;
    [SerializeField]
    private BasicItemData[] item;
    [SerializeField]
    private BasicItemData[] sellList;
    [SerializeField]
    private ItemExplain[] itemExplain;
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private PlayerStatus playerStatus;
    private void Awake()
    {
        SellingItem();
    }
    public void SellingItem()
    {
        int[] checkOverlap = new int[4];
        for (int i = 0; i < 4; i++)
        {
            bool isOverlap;
            do
            {
                isOverlap = false;
                int randomItem = Random.Range(0, sellList.Length);
                for (int j = 0; j < i; j++)
                {
                    if (randomItem == checkOverlap[j])
                    {
                        isOverlap = true;
                        break;
                    }
                }
                if (!isOverlap)
                {
                    item[i] = sellList[randomItem];
                    itemImage[i].sprite = item[i].ItemSprite;
                    itemExplain[i].sellingItem = item[i];
                    checkOverlap[i] = randomItem;
                }
            } while (isOverlap);
        }
    }
    public void EmptySlot(BasicItemData itemB)
    {
        Debug.Log("비우기 시작");
        for (int i = 0; i < item.Length; i++) {
            if (item[i] == itemB)
            {
                item[i]=null;
                itemImage[i].sprite = null;
                itemExplain[i].sellingItem = null;
                Debug.Log("비우기 끝");
            }
        }
    }

    public void BuyItem(BasicItemData item)
    {
        if (item.ItemPrice <= playerStatus.Gold)
        {
            Debug.Log(playerStatus.Gold + "-" + item.ItemPrice);
            playerStatus.Gold -= item.ItemPrice;
            Debug.Log("골드뺌");
            inventory.AddItem(item);
            Debug.Log("아이템 삼");
            EmptySlot(item);
        }
        else
            Debug.Log("골드가 부족합니다.");
    } 
}
