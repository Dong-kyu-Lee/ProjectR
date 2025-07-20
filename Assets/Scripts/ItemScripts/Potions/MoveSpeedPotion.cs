using UnityEngine;

[CreateAssetMenu(fileName = "New MoveSpeedPotion", menuName = "Item/Potion/MoveSpeed")]
public class MoveSpeedPotion : PotionItemData
{
    [SerializeField] private float moveSpeedIncreasement = 10f;
    public float MoveSpeedIncreasement => moveSpeedIncreasement;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.MoveSpeedIncrease;
    }
}
