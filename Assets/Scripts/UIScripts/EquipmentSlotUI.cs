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
            if (draggedSlot is EquipmentSlotUI)
            {
                parentUI.PlayerInventory.SwapEquipmentItemSlots(slotIndex, (draggedSlot as EquipmentSlotUI).slotIndex);
                SwapItemData(draggedSlot);
            }
            else //draggedSLot is InventorySlotUI
            {
                if (nowItemData.ItemType == ItemType.EQUIPMENT)
                {
                    parentUI.PlayerInventory.SwapEquippedItemWithInventory(slotIndex, draggedSlot.NowItemData as EquipmentItemData);
                    SwapItemData(draggedSlot);
                }
                else //nowItemData.ItemType == ItemType.Dummy
                {
                    parentUI.PlayerInventory.LoadEquipmentItemFromInventory(draggedSlot.NowItemData as EquipmentItemData, slotIndex);
                    SetItemData(draggedSlot.NowItemData,draggedSlot.ItemCount);
                    draggedSlot.DeleteItemData();
                }
            }
        }
    }
}
