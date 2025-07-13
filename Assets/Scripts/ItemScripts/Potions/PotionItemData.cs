using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Item/Potion")]
public class PotionItemData : ConsumableItemData
{
    public PotionType potionType;
    public float effectValue;

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        switch (potionType)
        {
            case PotionType.HpIncrease:
                playerStatus.Hp += effectValue;
                break;
            case PotionType.DamageIncrease:
                playerStatus.AdditionalDamage += effectValue / 100f;
                break;
            case PotionType.DamageReductionIncrease:
                playerStatus.AdditionalDamageReduction += effectValue / 100f;
                break;
            case PotionType.CriticalPercentIncrease:
                playerStatus.CriticalPercent += effectValue;
                break;
            case PotionType.CriticalDamageIncrease:
                playerStatus.CriticalDamage += effectValue / 100f;
                break;
            case PotionType.AttackSpeedIncrease:
                playerStatus.AdditionalAttackSpeed += effectValue / 100f;
                break;
            case PotionType.MoveSpeedIncrease:
                playerStatus.AdditionalMoveSpeed += effectValue / 100f;
                break;
            default:
                Debug.LogWarning($"[PotionItemData] 알 수 없는 포션 타입: {potionType}");
                break;
        }
    }
}
