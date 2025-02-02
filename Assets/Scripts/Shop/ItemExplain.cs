using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemExplain : MonoBehaviour
{
    [SerializeField]
    GameObject itemExplainUI;
    [SerializeField]
    TextMeshPro itemExTxt;      //부연설명
    [SerializeField]
    TextMeshPro itemEffectTxt;  //효과
    [SerializeField]
    TextMeshPro itemPriceTxt;  //가격
    [SerializeField]
    TextMeshPro itemName;   //아이템 이름
    [SerializeField]
    public BasicItemData sellingItem;  //매대의 아이템 
    [SerializeField]
    ItemSlotManager itemSlotManager;

    private void Awake()
    {
        itemExplainUI.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (itemExplainUI.activeSelf)
            {
                itemSlotManager.BuyItem(sellingItem);
                itemExplainUI.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (sellingItem.ItemName != "None")
            {
                ChangeInfo();
                itemExplainUI.SetActive(true);
            }
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
        itemPriceTxt.text = sellingItem.ItemPrice.ToString();
        itemEffectTxt.text = sellingItem.ItemDescription;
        itemExTxt.text = sellingItem.ItemExplain;
        itemName.text = sellingItem.ItemName;
    }
}
