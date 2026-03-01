using UnityEngine;

public enum ConsumableKind
{
    Potion,
    Throwable,
    ETC
}
[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/Consumable")]
public abstract class ConsumableItemData : BasicItemData
{
    public ConsumableKind kind;

    public abstract void ActivateItemEffect(PlayerStatus player);
}
