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
    SpriteRenderer exItemImage;   //설명지의 이미지
    [SerializeField]
    SpriteRenderer sellingItemImage;    //매대의 아이템 이미지
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
        exItemImage.sprite = sellingItemImage.sprite;
        /* 추후 아이템 설정되면 변경
         * itemPriceText.text = sellingItemImage.name;
         */
    }
}
