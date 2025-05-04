using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableBuffItem_Data", menuName = "Scriptable Object/Consumable Buff Item Data", order = 1)]
public class ConsumableBuffItem : ConsumableItemData
{
    [SerializeField]
    private BuffType buffType;      //플레이어에게 부여할 버프의 타입
    [SerializeField]
    private float buffDuration;     //플레이어에게 부여할 버프의 지속시간

    //아이템을 사용했을 경우 플레이어에게 버프를 부여하는 메서드
    public override void ActivateItemEffect(PlayerStatus player)
    {
        player.GetComponent<BuffManager>().ActivateBuff(buffType, buffDuration);
    }
}
