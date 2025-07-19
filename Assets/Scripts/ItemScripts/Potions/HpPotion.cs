using UnityEngine;

[CreateAssetMenu(fileName = "New HpPotion", menuName = "Item/Potion/HpPotion")]
public class HpPotion : PotionItemData
{
    [SerializeField] private float hpIncreasePercent = 30f;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.HpIncrease;
    }
}
