using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableBuffItem_Data", menuName = "Scriptable Object/Consumable Buff Item Data", order = 1)]
public class ConsumableBuffItem : ConsumableItemData
{
    [SerializeField]
    private BuffType buffType;
    [SerializeField]
    private float buffDuration;

    public override void ActivateItemEffect(PlayerStatus player)
    {
        player.GetComponent<PlayerBuffManager>().ActivateBuff(buffType, buffDuration);
    }
}
