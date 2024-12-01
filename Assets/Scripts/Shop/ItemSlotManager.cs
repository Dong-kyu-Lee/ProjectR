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
    private Sprite[] sellList;

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
                    itemImage[i].sprite = sellList[randomItem];
                    checkOverlap[i] = randomItem;
                }
            } while (isOverlap);
        }
    }
}
