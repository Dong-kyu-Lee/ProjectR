using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public abstract class ConsumableItemData : BasicItemData
{
    public Sprite ItemSprite { get { return itemSprite; } }
    public string ItemName { get { return itemName; } }
    public string ItemDescription { get { return itemDescription; } }
    //public int maxAmount = 0;

    //아이템 사용할 경우 효과를 발휘하게 해주는 메서드
    public abstract void ActivateItemEffect(GameObject player);
}