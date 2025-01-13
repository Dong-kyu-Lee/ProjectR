using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemExplain : MonoBehaviour
{
    [SerializeField]
    GameObject itemExplainUI;   
    [SerializeField]
    TextMeshPro itemExTxt;      //부연설명
    [SerializeField]
    TextMeshPro itemEffectTxt;  //효과
    [SerializeField]
    TextMeshPro itemPriceText;  //가격
    [SerializeField]
    BasicItemData sellingItem;  //매대의 아이템 
    [SerializeField]
    TextMeshPro itemName;   //아이템 이름
    private void Awake()
    {
        itemExplainUI.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ChangeInfo();
            itemExplainUI.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            itemExplainUI.SetActive(false);
        }
    }
    private void ChangeInfo()
    {
        itemPriceText.text = sellingItem.ItemPrice.ToString();
        itemEffectTxt.text = sellingItem.ItemDescription;
        itemExTxt.text = sellingItem.ItemExplain;
        itemName.text = sellingItem.ItemName;
    }
}
