using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlotUI : ItemSlotUI
{
    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject) return;
        ItemSlotUI draggedSlot = eventData.pointerDrag.GetComponent<ItemSlotUI>();
        
        if (draggedSlot.NowItemData.ItemType == ItemType.CONSUMABLE)
        {
            switch (NowItemData.ItemType)
            {
                case ItemType.CONSUMABLE:   //(인벤토리 <-> 퀵슬롯) 아이템 스왑 기능
                    parentUI.PlayerInventory.SwapQuickSlotWithInventory(draggedSlot.NowItemData as ConsumableItemData);
                    SwapItemData(draggedSlot);
                    break;
                case ItemType.DUMMY:    //(인벤토리 -> 퀵슬롯) 아이템 로드 기능
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
