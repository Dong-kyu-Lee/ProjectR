using UnityEngine;

[CreateAssetMenu(fileName = "New CriticalPercentPotion", menuName = "Item/Potion/CriticalPercent")]
public class CriticalPercentPotion : PotionItemData
{
    [SerializeField] private float criticalIncreasePercent = 10f;
    public float CriticalIncreasePercent => criticalIncreasePercent;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.CriticalPercentIncrease;
    }
}
