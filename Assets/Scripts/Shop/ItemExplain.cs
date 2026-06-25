using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemExplain : MonoBehaviour
{
    [SerializeField]
    GameObject itemExplainUI;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    TextMeshPro itemExTxt;      //부연설명
    [SerializeField]
    TextMeshPro itemEffectTxt;  //효과
    [SerializeField]
    TextMeshPro itemPriceTxt;  //가격
    [SerializeField]
    TextMeshPro itemName;   //아이템 이름
    [SerializeField]
    TextMeshPro itemGradeTxt;   // 등급
    [SerializeField]
    public BasicItemData item;  //아이템

    private void Awake()
    {
        itemExplainUI.SetActive(false);
    }

    // 씬에 스폰되자마자 아이템 데이터의 이미지로 외형을 동기화합니다.
    private void Start()
    {
        if (item != null && item.ItemName != "None" && spriteRenderer != null)
        {
            spriteRenderer.sprite = item.ItemSprite;
        }
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
        if (collision.CompareTag("Player"))
        {
            if (item != null && item.ItemName != "None")
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

    public void ChangeInfo()
    {
        //  UI 컴포넌트가 null일 때 발생하는 에러를 방지합니다.
        if (spriteRenderer != null) spriteRenderer.sprite = item.ItemSprite;
        if (itemName != null) itemName.text = item.ItemName;
        if (itemEffectTxt != null) itemEffectTxt.text = item.ItemDescription;
        if (itemExTxt != null) itemExTxt.text = item.ItemExplain;
        if (itemGradeTxt != null) itemGradeTxt.text = item.ItemGrade.ToString();
        if (itemPriceTxt != null) itemPriceTxt.text = $"{item.ItemPrice} G";
    }
}