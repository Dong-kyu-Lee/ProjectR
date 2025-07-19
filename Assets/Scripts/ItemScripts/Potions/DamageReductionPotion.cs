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
}
