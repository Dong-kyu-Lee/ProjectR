using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Basic_Item_Data", menuName = "Scriptable Object/Basic_Item_Data", order = 1)]
public class BasicItemData : ScriptableObject
{
    [SerializeField]
    protected string itemName;
    [SerializeField]
    protected string itemDescription;
    [SerializeField]
    protected int itemID;
    [SerializeField]
    protected Sprite itemSprite;

    public string ItemName { get { return itemName; } }
    public string ItemDescription { get { return itemDescription; } }
    public int ItemID { get { return itemID; } }
    public Sprite ItemSprite { get { return itemSprite; } }
}