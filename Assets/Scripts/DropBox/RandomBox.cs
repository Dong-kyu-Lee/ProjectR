using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBox : MonoBehaviour
{
    [SerializeField]
    private DropableItem dropableItem;
    [SerializeField]
    private List<BasicItemData> normalBox;
    [SerializeField]
    private List<BasicItemData> rareBox;
    [SerializeField]
    private List<BasicItemData> epicBox;
    [SerializeField]
    private List<BasicItemData> uniqueBox;
    [SerializeField]
    private List<BasicItemData> legendaryBox;

    void Start()
    {
        BoxGrade();
    }
    private void BoxGrade()
    {
        int boxGrade = Random.Range(0, 5);
        if (boxGrade==0)
        {
            this.gameObject.tag = "normal";
            DropItem(normalBox);
        }
        else if (boxGrade == 1)
        {
            this.gameObject.tag = "rare";
            DropItem(rareBox);
        }
        else if (boxGrade == 2)
        {
            this.gameObject.tag = "epic";
            DropItem(epicBox);
        }
        else if (boxGrade == 3)
        {
            this.gameObject.tag = "unique";
            DropItem(uniqueBox);
        }
        else if (boxGrade == 4)
        {
            this.gameObject.tag = "legendary";
            DropItem(legendaryBox);
        }
    }
    public void DropItem(List<BasicItemData> itemList)
    {
        int dropNum = Random.Range(4, 5);
        bool isOverlap = false;
        int[] checkOverlap = new int[dropNum];
        BasicItemData[] dropItem = new BasicItemData[dropNum];
        for (int i = 0; i < 4; i++)
        {
            do
            {
                isOverlap = false;
                int randomItem = Random.Range(0, itemList.Count);
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
                    //dropItem[i] = itemList[randomItem];
                    //itemImage[i].sprite = dropItem[i].ItemSprite;
                    //itemExplain[i].sellingItem = dropItem[i];
                    //checkOverlap[i] = randomItem;
                }
            } while (isOverlap);
        }
    }
}
