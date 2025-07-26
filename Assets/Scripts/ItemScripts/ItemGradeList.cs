using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGradeList", menuName = "Data/ItemGradeList")]
public class ItemGradeList : ScriptableObject
{
    [Header("아이템 등급별 리스트")]
    public List<BasicItemData> normalItem;
    public List<BasicItemData> rareItem;
    public List<BasicItemData> epicItem;
    public List<BasicItemData> uniqueItem;
    public List<BasicItemData> legendaryItem;

    public List<BasicItemData> GetListByGrade(ItemGrade grade)
    {
        switch (grade)
        {
            case ItemGrade.Normal: return normalItem;
            case ItemGrade.Rare: return rareItem;
            case ItemGrade.Epic: return epicItem;
            case ItemGrade.Unique: return uniqueItem;
            case ItemGrade.Legendary: return legendaryItem;
            default: return normalItem;
        }
    }
}
