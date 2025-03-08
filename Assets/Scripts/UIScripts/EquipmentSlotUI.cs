using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : ItemSlotUI
{
    public override void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI draggedSlot = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (draggedSlot.NowItemData.ItemType == ItemType.EQUIPMENT)
        {
            if (draggedSlot is EquipmentSlotUI) //장비 칸끼리 아이템 스왑
            {
                parentUI.PlayerInventory.SwapEquipmentItemSlots(slotIndex, (draggedSlot as EquipmentSlotUI).slotIndex);
                SwapItemData(draggedSlot);
            }
            else //draggedSLot is InventorySlotUI
            {
                if (nowItemData.ItemType == ItemType.EQUIPMENT) // 장비 <-> 인벤토리) 장비 스왑
                {
                    parentUI.PlayerInventory.SwapEquippedItemWithInventory(slotIndex, draggedSlot.NowItemData as EquipmentItemData);
                    SwapItemData(draggedSlot);
                }
                else //nowItemData.ItemType == ItemType.Dummy // 장비칸 <- 인벤토리) 장비 로드
                {
                    parentUI.PlayerInventory.LoadEquipmentItemFromInventory(draggedSlot.NowItemData as EquipmentItemData, slotIndex);
                    SetItemData(draggedSlot.NowItemData,draggedSlot.ItemCount);
                    draggedSlot.DeleteItemData();
                }
            }
        }
    }
}
