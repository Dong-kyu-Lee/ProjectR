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

    // Update is called once per frame
    void Update()
    {
        
    }
    private void BoxGrade()
    {
        if (this.gameObject.tag == "normal")
        {
            DropItem(normalBox);
        }
        if (this.gameObject.tag == "rare")
        {
            DropItem(rareBox);
        }
        if (this.gameObject.tag == "epic")
        {
            DropItem(epicBox);
        }
        if (this.gameObject.tag == "unique")
        {
            DropItem(uniqueBox);
        }
        if (this.gameObject.tag == "legendary")
        {
            DropItem(legendaryBox);
        }
    }
    public void DropItem(List<BasicItemData> itemList)
    {
        int[] checkOverlap = new int[4];
        for (int i = 0; i < 4; i++)
        {
            //do
            //{
            //    isOverlap = false;
            //    int randomItem = Random.Range(0, itemList.Length);
            //    for (int j = 0; j < i; j++)
            //    {
            //        if (randomItem == checkOverlap[j])
            //        {
            //            isOverlap = true;
            //            break;
            //        }
            //    }
            //    if (!isOverlap)
            //    {
            //        item[i] = itemList[randomItem];
            //        itemImage[i].sprite = item[i].ItemSprite;
            //        itemExplain[i].sellingItem = item[i];
            //        checkOverlap[i] = randomItem;
            //    }
            //} while (isOverlap);
        }
    }
}
