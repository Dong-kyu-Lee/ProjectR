using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : ItemSlotUI
{
    public override void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI draggedSlot = eventData.pointerDrag.GetComponent<ItemSlotUI>();
        if (draggedSlot == null || draggedSlot == this || parentUI.PlayerInventory == null) return;

        if (draggedSlot.NowItemData?.ItemType == ItemType.EQUIPMENT)
        {
            if (draggedSlot is EquipmentSlotUI)
            {
                parentUI.PlayerInventory.SwapEquipmentItemSlots(this.slotIndex, draggedSlot.SlotIndex);
            }
            else
            {
                parentUI.PlayerInventory.SwapEquippedItemWithInventory(this.slotIndex, draggedSlot.SlotIndex);
            }

            parentUI.RefreshInventoryUI(); // 모든 변화 후 UI 일괄 갱신됨
        }
    }
}