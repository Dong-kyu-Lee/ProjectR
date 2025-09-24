using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItem
{
    public GameObject prefab;
    public float dropRate;
    public int maxCount = 1;
}

[CreateAssetMenu(fileName = "LootTable", menuName = "Scriptable Object/Loot Table")]
public class LootTable : ScriptableObject
{
    public DropItem[] dropItems;
}
