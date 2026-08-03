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
    TextMeshPro howToGetItemTxt;
    [SerializeField]
    TextMeshPro itemExTxt;      // 부연설명
    [SerializeField]
    TextMeshPro itemEffectTxt;  // 효과
    [SerializeField]
    TextMeshPro itemPriceTxt;   // 가격
    [SerializeField]
    TextMeshPro itemName;       // 아이템 이름
    [SerializeField]
    TextMeshPro itemGradeTxt;   // 등급
    [SerializeField]
    public BasicItemData item;  // 아이템

    private void Awake()
    {
        // 묶어둔 함수를 사용하여 초기화
        SetUIActive(false);
    }

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
        SetUIActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (item != null && item.ItemName != "None")
            {
                ChangeInfo();
                SetUIActive(true); // 같이 켜짐
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetUIActive(false); // 같이 꺼짐
        }
    }

    public void ChangeInfo()
    {
        if (spriteRenderer != null) spriteRenderer.sprite = item.ItemSprite;
        if (itemName != null) itemName.text = item.ItemName;
        if (itemEffectTxt != null) itemEffectTxt.text = item.ItemDescription;
        if (itemExTxt != null) itemExTxt.text = item.ItemExplain;
        if (itemGradeTxt != null) itemGradeTxt.text = item.ItemGrade.ToString();
        if (itemPriceTxt != null) itemPriceTxt.text = $"{item.ItemPrice} G";
    }

    private void SetUIActive(bool isActive)
    {
        if (itemExplainUI != null)
            itemExplainUI.SetActive(isActive);

        if (howToGetItemTxt != null)
            howToGetItemTxt.gameObject.SetActive(isActive);
    }
}