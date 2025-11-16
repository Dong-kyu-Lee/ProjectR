using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : ItemSlotUI
{
    public override void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI draggedSlot = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        // 유효성 검사
        if (draggedSlot == null || draggedSlot == this) return; // 자기 자신 드롭 방지
        if (parentUI.PlayerInventory == null) return;

        if (draggedSlot.NowItemData.ItemType == ItemType.EQUIPMENT)
        {
            if (draggedSlot is EquipmentSlotUI) //장비 칸끼리 아이템 스왑
            {
                parentUI.PlayerInventory.SwapEquipmentItemSlots(slotIndex, (draggedSlot as EquipmentSlotUI).slotIndex);
            }
            else 
            {
                if (nowItemData.ItemType == ItemType.EQUIPMENT) // (장비 <-> 인벤토리) => 장비칸 스왑
                {
                    parentUI.PlayerInventory.SwapEquippedItemWithInventory(slotIndex, draggedSlot.NowItemData as EquipmentItemData);

                }
                else  // (인벤토리 -> 장비칸) => 장비 로드
                {
                    // NoRefresh 함수 호출
                    parentUI.PlayerInventory.LoadEqFromInv_NoRefresh(
                        draggedSlot.NowItemData as EquipmentItemData,
                        slotIndex
                    );

                    //  NoRefresh 함수를 썼으므로 Refresh 호출
                    parentUI.RefreshInventoryUI();
                }
            }
        }
    }
}