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
    public BasicItemData item;  //아이템

    private void Awake()
    {
        itemExplainUI.SetActive(false);
    }
    public bool IsActive()
    {
        return itemExplainUI.activeSelf;
    }
    public void HideUI()
    {
        itemExplainUI.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (item.ItemName != "None")
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
        itemPriceTxt.text = item.ItemPrice.ToString();
        itemEffectTxt.text = item.ItemDescription;
        itemExTxt.text = item.ItemExplain;
        itemName.text = item.ItemName;
    }
}
