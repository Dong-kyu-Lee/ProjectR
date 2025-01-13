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
    private Inventory inventory;
    private PlayerStatus playerStatus;


    private void Awake()
    {
        SellingItem();
        inventory = GetComponent<Inventory>();
        playerStatus = GetComponent<PlayerStatus>();
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
                    checkOverlap[i] = randomItem;
                }
            } while (isOverlap);
        }
    }
    public void EmptySlot()
    {
        
    }

    public void BuyItem(BasicItemData item)
    {
        inventory.AddItem(item);
        playerStatus.Gold -= item.ItemPrice;
    } 
}
