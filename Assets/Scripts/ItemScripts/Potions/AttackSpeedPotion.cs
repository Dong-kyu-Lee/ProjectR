using UnityEngine;

[CreateAssetMenu(fileName = "New AttackSpeedPotion", menuName = "Item/Potion/AttackSpeed")]
public class AttackSpeedPotion : PotionItemData
{
    [SerializeField] private float attackSpeedIncreasement = 10f;
    public float AttackSpeedIncreasement => attackSpeedIncreasement;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.AttackSpeedIncrease;
    }
}
