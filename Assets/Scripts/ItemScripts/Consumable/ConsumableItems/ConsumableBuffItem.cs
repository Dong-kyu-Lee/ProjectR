using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableBuffItem_Data", menuName = "Scriptable Object/ConsumableBuffItem_Data", order = 1)]
public class ConsumableBuffItem : ConsumableItemData
{
    [SerializeField]
    private BuffType buffType;
    [SerializeField]
    private float buffDuration;

    public override void ActivateItemEffect(GameObject player)
    {
        player.GetComponent<PlayerBuffManager>().ActivateBuff(buffType, buffDuration);
    }
}
