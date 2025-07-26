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
}
