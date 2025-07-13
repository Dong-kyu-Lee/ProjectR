using UnityEngine;

[CreateAssetMenu(fileName = "New DamagePotion", menuName = "Item/Potion/Damage")]
public class DamagePotion : PotionItemData
{
    [SerializeField] private float damageIncreasePercent = 10f;
    public float DamageIncreasePercent => damageIncreasePercent;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.DamageIncrease;
    }

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float before = playerStatus.AdditionalDamage;
        playerStatus.AdditionalDamage += damageIncreasePercent / 100f;
        Debug.Log($"[DamagePotion] 추가 피해량 {damageIncreasePercent}% 증가: {before} → {playerStatus.AdditionalDamage}");
    }
}
