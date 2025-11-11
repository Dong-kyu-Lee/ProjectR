using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject equipSlotParentObj;
    [SerializeField] private EquipmentSlotUI[] equipSlotImgs = null;

    [SerializeField] private GameObject inventorySlotParentObj;
    [SerializeField] private ItemSlotUI[] inventorySlotImgs;

    [SerializeField] private ItemSlotUI quickSlotImg;
    public ItemSlotUI QuickSlotImg { get { return quickSlotImg; } }

    [SerializeField] private ItemSlotUI previewSlotUI;
    public ItemSlotUI PreviewSlotUI { get { return previewSlotUI; } }

    [SerializeField] private Inventory playerInventory;
    public Inventory PlayerInventory { get { return playerInventory; } }

    public void Init()
    {
        playerInventory = GameManager.Instance.CurrentPlayer.transform.GetChild(0).GetComponent<Inventory>();
        playerInventory.MyInventoryUI = this;

        InitiateAllItemsSlots();
        Inventory.OnItemAdded += HandleItemAdded;

        playerInventory.UpdateQuickSlotReference();
    }

    // 인벤토리 전체 UI를 다시 갱신하는 함수
    public void RefreshInventoryUI()
    {
        if (playerInventory == null || inventorySlotImgs == null)
            return;

        // 인벤토리의 모든 아이템 슬롯 데이터를 가져옴
        var slots = playerInventory.InventorySlots;

        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            if (i < slots.Count)
            {
                var slot = slots[i];
                if (slot.itemData != null)
                {
                    inventorySlotImgs[i].SetItemData(slot.itemData, slot.count);
                }
                else
                {
                    inventorySlotImgs[i].DeleteItemData();
                }
            }
            else
            {
                // 남은 슬롯은 더미로 채우는것
                inventorySlotImgs[i].DeleteItemData();
            }
        }

        // 퀵슬롯은 인벤토리 첫 번째 칸을 참조하므로 함께 업데이트
        playerInventory.UpdateQuickSlotReference();
    }


    private void HandleItemAdded(BasicItemData item, int amount)
    {
        SetItemToUI(item, amount);
    }

    private void InitiateAllItemsSlots()
    {
        previewSlotUI.Init(gameObject, -1);

        equipSlotImgs = equipSlotParentObj.transform.GetComponentsInChildren<EquipmentSlotUI>();
        for (int i = 0; i < equipSlotImgs.Length; i++)
        {
            equipSlotImgs[i].Init(gameObject, i);
        }

        inventorySlotImgs = inventorySlotParentObj.transform.GetComponentsInChildren<ItemSlotUI>();
        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            inventorySlotImgs[i].Init(gameObject, i);
        }

        QuickSlotImg.Init(gameObject, -1);
    }

    public void SetItemToUI(BasicItemData item, int amount)
    {
        switch (item.ItemType)
        {
            case ItemType.CONSUMABLE:
                SetInventorySlotData(item, amount);
                break;
            case ItemType.EQUIPMENT:
                SetEquippedItemSlotData(item as EquipmentItemData);
                break;
        }
    }

    public void SetEquippedItemSlotData(EquipmentItemData itemData)
    {
        for (int i = 0; i < equipSlotImgs.Length; i++)
        {
            if (equipSlotImgs[i].NowItemData.ItemType == ItemType.DUMMY)
            {
                equipSlotImgs[i].SetItemData(itemData, 1);
                return;
            }
        }
        SetInventorySlotData(itemData, 1);
    }

    public void SetInventorySlotData(BasicItemData itemData, int amount)
    {
        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            if (inventorySlotImgs[i].NowItemData.ItemType == ItemType.DUMMY)
            {
                inventorySlotImgs[i].SetItemData(itemData, amount);
                return;
            }
        }
    }

    public void UpdateItemSlotAmount(BasicItemData itemData, int amount)
    {
        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            if (inventorySlotImgs[i].NowItemData == itemData)
            {
                inventorySlotImgs[i].SetItemData(itemData, amount);
                Debug.Log("아이템 갯수 UI에 반영");
                return;
            }
        }
    }

    public void SetQuickSLotItemData(BasicItemData itemData, int amount)
    {
        if (!quickSlotImg.IsInitialized)
            quickSlotImg.Init(gameObject, -1);

        quickSlotImg.SetItemData(itemData, amount);
    }

    private void OnDestroy()
    {
        Inventory.OnItemAdded -= HandleItemAdded;
    }

    // 인벤토리 슬롯 인덱스로 해당 UI 슬롯을 반환하는 함수
    public ItemSlotUI GetInventorySlotUI(int index)
    {
        if (inventorySlotImgs == null) return null;
        if (index < 0 || index >= inventorySlotImgs.Length) return null;
        return inventorySlotImgs[index];
        }

     //이미 존재하는 아이템 슬롯의 수량만 갱신하는 함수(새 슬롯 생성 방지)
    public void UpdateExistingItemSlot(BasicItemData itemData, int newAmount)
    {
        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            var slotItem = inventorySlotImgs[i].NowItemData;
            if (slotItem != null && slotItem.ItemName == itemData.ItemName)
            {
                inventorySlotImgs[i].SetItemAmountData(newAmount);
                Debug.Log($"[InventoryUI] {itemData.ItemName} 수량 {newAmount}로 갱신됨");
                return;
            }
        }

        // 만약 기존 슬롯이 없으면 새 슬롯 생성 (안전장치)
        SetInventorySlotData(itemData, newAmount);
    }

}
