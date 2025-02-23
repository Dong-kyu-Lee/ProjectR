using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] itemImage;
    [SerializeField]
    private BasicItemData[] item;
    [SerializeField]
    private DropableItem dropableItem;
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
                    int randomItem = Random.Range(0, dropableItem.dropableItem.Count);
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
                        item[i] = dropableItem.dropableItem[randomItem];
                        itemImage[i].sprite = item[i].ItemSprite;
                        itemExplain[i].item = item[i];
                        checkOverlap[i] = randomItem;
                    }
                } while (isOverlap);
            }
            else if (item[i] == noneItem)
            {
                continue;
            }
        }
    }
    public void EmptySlot(BasicItemData itemB)
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] == itemB)
            {
                item[i] = noneItem;
                itemImage[i].sprite = null;
                itemExplain[i].item = noneItem;
            }
        }
    }

    public void BuyItem(BasicItemData item)
    {
        if (item.ItemPrice <= playerStatus.Gold)
        {
            playerStatus.Gold -= item.ItemPrice;
            EmptySlot(item);
            inventory.AddItem(item);
            Debug.Log(item.ItemName);
            dropableItem.removeItem(item);
        }
        else
            Debug.Log("골드가 부족합니다.");
    }
}
