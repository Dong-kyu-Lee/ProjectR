using UnityEngine;

[CreateAssetMenu(fileName = "New DamageReductionPotion", menuName = "Item/Potion/DamageReduction")]
public class DamageReductionPotion : PotionItemData
{
    [SerializeField] private float damageReductionPercent = 10f;
    public float DamageReductionPercent => damageReductionPercent;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.DamageReductionIncrease;
    }

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float before = playerStatus.AdditionalDamageReduction;
        playerStatus.AdditionalDamageReduction += damageReductionPercent / 100f;
        Debug.Log($"[DamageReductionPotion] 피해 감소 {damageReductionPercent}% 증가: {before} → {playerStatus.AdditionalDamageReduction}");
    }
}
