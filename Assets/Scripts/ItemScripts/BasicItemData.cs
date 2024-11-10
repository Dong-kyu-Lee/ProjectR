using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item_Scriptable", menuName = "Item_Scriptable_Data")]
public class BasicItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int itemID;
    public Sprite itemSprite;
}