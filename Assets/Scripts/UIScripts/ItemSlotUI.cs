using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private BasicItemData dummyItemData;   //더미 아이템 데이터(= Null ItemData)
    [SerializeField] private BasicItemData nowItemData; //현재 아이템 데이터
    private Image itemSlotImage;
    public Image ItemSlotImage { get; set; }
    public BasicItemData NowItemData { get; set; }

    private void Awake()
    {
        nowItemData = dummyItemData;
        itemSlotImage = GetComponent<Image>();
        itemSlotImage.sprite = nowItemData.ItemSprite;
    }

    //자신의 아이템 데이터를 설정하는 메서드
    public void SetItemData(BasicItemData itemData) 
    {
        nowItemData = itemData;
        UpdateItemSprite();
    }

    //자신의 슬롯의 아이템 이미지를 업데이트하는 메서드
    public void UpdateItemSprite()
    {
        itemSlotImage.sprite = nowItemData.ItemSprite;
    }

    //더미 아이템 데이터로 설정하고 자신의 슬롯의 아이템 이미지를 초기화하는 메서드
    public void DeleteItemData()
    {
        nowItemData = dummyItemData;
        UpdateItemSprite();
    }
}
