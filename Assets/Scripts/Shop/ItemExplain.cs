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
        Debug.Log("콜라이더에 들어옴");
        if (collision.tag == "Player")
        {
            Debug.Log("들어옴");
            ChangeInfo();
            itemExplainUI.SetActive(true);
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("콜라이더에 들어옴");
    //    if (collision.gameObject.name =="Player")
    //    {
    //        Debug.Log("들어옴");
    //        ChangeInfo();
    //        itemExplainUI.SetActive(true);
    //    }
    //}
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("콜라이더에서 나감");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("나감");
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
