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

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float before = playerStatus.CriticalPercent;
        playerStatus.CriticalPercent += criticalIncreasePercent;
        Debug.Log($"[CriticalPercentPotion] 크리티컬 확률 {criticalIncreasePercent}% 증가: {before} → {playerStatus.CriticalPercent}");
    }
}
