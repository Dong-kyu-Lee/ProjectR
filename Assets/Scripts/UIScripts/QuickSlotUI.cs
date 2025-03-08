using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlotUI : ItemSlotUI
{
    public override void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI draggedSlot = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (draggedSlot.NowItemData.ItemType == ItemType.CONSUMABLE)
        {
            switch (NowItemData.ItemType)
            {
                case ItemType.CONSUMABLE:   //스왑
                    parentUI.PlayerInventory.SwapQuickSlotWithInventory(draggedSlot.NowItemData as ConsumableItemData);
                    SwapItemData(draggedSlot);
                    break;
                case ItemType.DUMMY:    //로드
                    parentUI.PlayerInventory.LoadToQuickSlotFromInventory(draggedSlot.NowItemData as ConsumableItemData);
                    SetItemData(draggedSlot.NowItemData, draggedSlot.ItemCount);
                    draggedSlot.DeleteItemData();
                    break;
                default:
                    break;
            }
        }
    }
}
