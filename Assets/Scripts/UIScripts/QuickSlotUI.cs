using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlotUI : ItemSlotUI
{
    // QuickSlot 에 들어가는 실제 데이터
    private Inventory inventory;

    public override void Init(GameObject parent, int indexNumber)
    {
        base.Init(parent, indexNumber);
        inventory = parentUI.PlayerInventory;
    }

    // QuickSlot은 드래그해서 이동 불가
    public override void OnDrop(PointerEventData eventData)
    {
        return;
    }

    // 퀵슬롯 클릭 = 아이템 사용
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventory.UseQuickSlotItem();
        }
    }

    // Inventory.UpdateQuickSlotReference()가 호출하면 여기서 UI 갱신
    public void UpdateQuickSlot(BasicItemData itemData, int amount)
    {
        // 순수하게 UI만 갱신합니다.
        /*
        if (itemData != null && itemData.ItemType == ItemType.CONSUMABLE)
        {
            inventory.SetQuickSlot(itemData, amount); 
        }
        */

        // UI 갱신
        SetItemData(itemData, amount);
    }
}