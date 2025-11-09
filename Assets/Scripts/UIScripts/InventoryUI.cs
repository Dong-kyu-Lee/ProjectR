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

    private void SetEquippedItemSlotData(EquipmentItemData itemData)
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

}
