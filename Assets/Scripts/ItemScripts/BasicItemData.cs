using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    CONSUMABLE,
    EQUIPMENT
}
[CreateAssetMenu(fileName = "Basic_Item_Data", menuName = "Scriptable Object/Basic_Item_Data", order = 1)]
public class BasicItemData : ScriptableObject
{
    [SerializeField]
    protected string itemName;  //아이템 이름
    
    [SerializeField, Multiline]
    protected string itemDescription;   //아이템 설명문
    [SerializeField]
    protected int itemID;       //아이템 ID
    [SerializeField]
    protected Sprite itemSprite;    //아이템 스프라이트
    [SerializeField]
    protected int maxAmount = 99;   //한번의 최대 보유량
    [SerializeField]
    protected ItemType itemType;    //아이템 타입

    public string ItemName { get { return itemName; } }
    public string ItemDescription { get { return itemDescription; } }
    public int ItemID { get { return itemID; } }
    public Sprite ItemSprite { get { return itemSprite; } }
    public int MaxAmount { get { return maxAmount; } }
    public ItemType ItemType { get { return itemType; } }
}