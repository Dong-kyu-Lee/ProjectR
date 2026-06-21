using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject equipSlotParentObj;
    [SerializeField] private EquipmentSlotUI[] equipSlotImgs = null;

    [SerializeField] private GameObject inventorySlotParentObj;
    [SerializeField] private ItemSlotUI[] inventorySlotImgs;

    [SerializeField] private QuickSlotUI quickSlotImg;
    public QuickSlotUI QuickSlotImg { get { return quickSlotImg; } }

    [SerializeField] private ItemSlotUI previewSlotUI;
    public ItemSlotUI PreviewSlotUI { get { return previewSlotUI; } }

    [SerializeField] private Inventory playerInventory;
    public Inventory PlayerInventory { get { return playerInventory; } }

    public void Init()
    {
        if (InGameUIManager.Instance != null && InGameUIManager.Instance.transform.root != this.transform.root)
        {
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
        {
            return;
        }

        // [수정/제거] playerInventory = GameManager.Instance.CurrentPlayer.transform.GetChild(0).GetComponent<Inventory>();
        // [추가] 자식 인덱스 순서에 의존하지 않고 안전하게 탐색하도록 변경
        playerInventory = GameManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();

        if (playerInventory != null)
        {
            playerInventory.MyInventoryUI = this;
            InitiateAllItemsSlots();

            RefreshInventoryUI();

            playerInventory.UpdateQuickSlotReference();
        }
        else
        {
            Debug.LogWarning("InventoryUI: 플레이어 인벤토리를 찾을 수 없습니다.");
        }
    }

    public void RefreshInventoryUI()
    {
        if (playerInventory == null)
            return;

        if (inventorySlotImgs != null)
        {
            var slots = playerInventory.InventorySlots;
            for (int i = 0; i < inventorySlotImgs.Length; i++)
            {
                if (i < slots.Count)
                {
                    var slot = slots[i];
                    if (slot.itemData != null && slot.itemData.ItemType != ItemType.DUMMY)
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
                    inventorySlotImgs[i].DeleteItemData();
                }
            }
        }

        if (equipSlotImgs != null)
        {
            var equipSlots = playerInventory.EquipmentItemSlot;
            for (int i = 0; i < equipSlotImgs.Length; i++)
            {
                if (i < equipSlots.Length)
                {
                    var itemData = equipSlots[i];
                    if (itemData != null && itemData.ItemType != ItemType.DUMMY)
                    {
                        equipSlotImgs[i].SetItemData(itemData, 1);
                    }
                    else
                    {
                        equipSlotImgs[i].DeleteItemData();
                    }
                }
                else
                {
                    equipSlotImgs[i].DeleteItemData();
                }
            }
        }

        playerInventory.UpdateQuickSlotReference();
    }


    private void InitiateAllItemsSlots()
    {
        if (previewSlotUI != null)
            previewSlotUI.Init(gameObject, -1);

        if (equipSlotParentObj != null)
        {
            equipSlotImgs = equipSlotParentObj.transform.GetComponentsInChildren<EquipmentSlotUI>();
            for (int i = 0; i < equipSlotImgs.Length; i++)
            {
                equipSlotImgs[i].Init(gameObject, i);
            }
        }

        if (inventorySlotParentObj != null)
        {
            inventorySlotImgs = inventorySlotParentObj.transform.GetComponentsInChildren<ItemSlotUI>();
            for (int i = 0; i < inventorySlotImgs.Length; i++)
            {
                inventorySlotImgs[i].Init(gameObject, i);
            }
        }

        if (quickSlotImg != null)
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
        if (equipSlotImgs == null) return;

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
        if (inventorySlotImgs == null) return;

        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            if (inventorySlotImgs[i].NowItemData.ItemType == ItemType.DUMMY)
            {
                inventorySlotImgs[i].SetItemData(itemData, amount);

                if (i == 0 && quickSlotImg != null)
                {
                    quickSlotImg.UpdateQuickSlot(itemData, amount);
                }
                return;
            }
        }
    }

    public void UpdateItemSlotAmount(BasicItemData itemData, int amount)
    {
        if (inventorySlotImgs == null) return;

        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            if (inventorySlotImgs[i].NowItemData == itemData)
            {
                inventorySlotImgs[i].SetItemData(itemData, amount);

                if (i == 0 && quickSlotImg != null)
                {
                    quickSlotImg.UpdateQuickSlot(itemData, amount);
                }

                return;
            }
        }
    }

    public void SetQuickSLotItemData(BasicItemData itemData, int amount)
    {
        if (quickSlotImg == null) return;

        if (!quickSlotImg.IsInitialized)
            quickSlotImg.Init(gameObject, -1);

        quickSlotImg.UpdateQuickSlot(itemData, amount);
    }

    public ItemSlotUI GetInventorySlotUI(int index)
    {
        if (inventorySlotImgs == null) return null;
        if (index < 0 || index >= inventorySlotImgs.Length) return null;
        return inventorySlotImgs[index];
    }

    public void UpdateExistingItemSlot(BasicItemData itemData, int newAmount)
    {
        if (inventorySlotImgs == null) return;

        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            var slotItem = inventorySlotImgs[i].NowItemData;
            if (slotItem != null && slotItem.ItemName == itemData.ItemName)
            {
                inventorySlotImgs[i].SetItemAmountData(newAmount);

                if (i == 0 && quickSlotImg != null)
                {
                    quickSlotImg.UpdateQuickSlot(itemData, newAmount);
                }

                return;
            }
        }

        SetInventorySlotData(itemData, newAmount);
    }
}