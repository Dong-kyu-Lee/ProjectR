using UnityEngine;

[CreateAssetMenu(fileName = "New CriticalDamagePotion", menuName = "Item/Potion/CriticalDamage")]
public class CriticalDamagePostion : PotionItemData
{
    [SerializeField] private float criticalDamageIncrease = 10f; // 증가량 (%)
    public float CriticalDamageIncrease => criticalDamageIncrease;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.CriticalDamageIncrease;
    }

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float before = playerStatus.CriticalDamage;
        playerStatus.CriticalDamage += playerStatus.CriticalDamage * (criticalDamageIncrease / 100f);
        Debug.Log($"[CriticalDamagePotion] 크리티컬 피해량 {criticalDamageIncrease}% 증가: {before} → {playerStatus.CriticalDamage}");
    }
}
