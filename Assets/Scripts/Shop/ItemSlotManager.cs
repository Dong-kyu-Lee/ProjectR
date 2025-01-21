using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using Unity.VisualScripting;
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
    private BasicItemData noneItem;
    [SerializeField]
    private ItemExplain[] itemExplain;
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private PlayerStatus playerStatus;
    bool isOverlap;
    private void Awake()
    {
        SellingItem();
    }
    public void SellingItem()
    {
        int[] checkOverlap = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (item[i] != noneItem)
            {
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
            else if(item[i]==noneItem) 
            {
                continue;
            }
        }
    }
    public void EmptySlot(BasicItemData itemB)
    {
        for (int i = 0; i < item.Length; i++) {
            if (item[i] == itemB)
            {
                item[i]=noneItem;
                itemImage[i].sprite = null;
                itemExplain[i].sellingItem = noneItem;
            }
        }
    }
    public void BuyItem(BasicItemData item)
    {
        if (item.ItemPrice <= playerStatus.Gold)
        {
            playerStatus.Gold -= item.ItemPrice;
            inventory.AddItem(item);
            EmptySlot(item);
        }
        else
            Debug.Log("골드가 부족합니다.");
    } 
}
