using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public BasicItemData itemData;
    public int count;

    public InventorySlot(BasicItemData data, int cnt)
    {
        itemData = data;
        count = cnt;
    }

    // 슬롯을 비우는 함수
    public void Clear(BasicItemData dummy)
    {
        itemData = dummy;
        count = 0;
    }
}

public class Inventory : MonoBehaviour
{
    public static event Action<BasicItemData, int> OnItemAdded;

    // 인벤토리 관련
    [SerializeField] private int maxInventorySlot = 12;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private List<InventorySlot> inventorySlots;

    // 소모품 관련 칸
    private ConsumableItemData quickSlot = null;
    private int quickSlotItemAmount = 0;

    // 장비 관련 칸
    [SerializeField] private int maxEquipSlot = 6;
    [SerializeField] private EquipmentItemData[] equipmentItemSlot;

    // 그 외
    [SerializeField] private EquipmentItemData dummyItemData;
    [SerializeField] private BasicItemData dummyInventoryItemData; //인벤토리용 더미
    [SerializeField] private InventoryUI myInventoryUI;

    public int QuickSlotItemAmount
    {
        get => quickSlotItemAmount;
        set => quickSlotItemAmount = Mathf.Max(0, value);
    }
    public ConsumableItemData QuickSlot => quickSlot;
    public int QuickSlotAmount => quickSlotItemAmount;
    public List<InventorySlot> InventorySlots => inventorySlots;
    public int MaxInventorySlot => maxInventorySlot;
    public int MaxEquipSlot => maxEquipSlot;
    public EquipmentItemData[] EquipmentItemSlot => equipmentItemSlot;
    public InventoryUI MyInventoryUI { get => myInventoryUI; set => myInventoryUI = value; }

    private void Awake()
    {
        inventorySlots = new List<InventorySlot>(maxInventorySlot);
        playerStatus = GetComponentInParent<PlayerStatus>();

        for (int i = 0; i < maxInventorySlot; i++)
        {
            inventorySlots.Add(new InventorySlot(dummyInventoryItemData, 0));
        }

        equipmentItemSlot = new EquipmentItemData[maxEquipSlot];
        for (int i = 0; i < equipmentItemSlot.Length; i++)
            equipmentItemSlot[i] = dummyItemData;
    }

    // 퀵슬롯에 있는 아이템을 사용하는 메서드.
    public void UseQuickSlotItem()
    {
        UseInventoryItem(0);
    }

    // 지정된 인덱스의 인벤토리 아이템을 사용하는 함수
    public void UseInventoryItem(int slotIndex)
    {
        if (inventorySlots == null || slotIndex < 0 || slotIndex >= inventorySlots.Count)
        {
            Debug.LogWarning($"[Inventory] 잘못된 인덱스 {slotIndex}의 아이템을 사용하려 했습니다.");
            return;
        }

        InventorySlot slot = inventorySlots[slotIndex];

        if (slot.itemData != null && slot.itemData.ItemType == ItemType.CONSUMABLE && slot.count > 0)
        {
            ConsumableItemData consumable = slot.itemData as ConsumableItemData;

            consumable.ActivateItemEffect(playerStatus);
            slot.count--; // 수량 1 감소

            if (slot.count <= 0)
            {
                // 아이템을 다 썼으면 슬롯을 비움 (더미 아이템으로 교체)
                slot.Clear(dummyInventoryItemData);
            }

            // UI 갱신 (퀵슬롯과 인벤토리 슬롯 모두 갱신)
            myInventoryUI.RefreshInventoryUI();
        }
        else
        {
            Debug.Log($"{slotIndex}번 슬롯에 사용할 아이템이 없습니다.");
        }
    }

    // 인벤토리에 있는 아이템 데이터를 퀵슬롯에 로드하는 메서드
    public void LoadToQuickSlotFromInventory(BasicItemData item)
    {
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == item);
        if (slot == null) return;

        quickSlot = item as ConsumableItemData;
        quickSlotItemAmount = slot.count;

        // 더미 아이템으로 '교체'
        slot.Clear(dummyInventoryItemData);

        myInventoryUI.RefreshInventoryUI(); // UI 갱신
    }

    // 아이템 획득 처리 (소모품/장비에 따라 다른 함수 실행)
    public bool AddItem(BasicItemData item, int amount = 1)
    {
        bool success = false;

        switch (item.ItemType)
        {
            case ItemType.CONSUMABLE:
                success = AddConsumableItem(item as ConsumableItemData, amount);
                break;
            case ItemType.EQUIPMENT:
                success = AddEquipmentItem(item as EquipmentItemData);
                break;
            case ItemType.DUMMY:
                Debug.LogWarning($"[Inventory] DUMMY 아이템은 추가하지 않습니다: {item.ItemName}");
                return false;
            default:
                Debug.LogWarning($"[Inventory] 정의되지 않은 아이템 타입: {item.ItemType}");
                return false;
        }

        if (success)
            OnItemAdded?.Invoke(item, amount);

        return success;
    }

    // 획득한 소모품 아이템 데이터를 인벤토리에 추가하는 메서드
    private bool AddConsumableItem(ConsumableItemData item, int amount)
    {
        // 1. 이미 같은 아이템이 있을 경우 수량만 증가
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == item);
        if (slot != null)
        {
            int newAmount = slot.count + amount;
            if (newAmount <= item.MaxAmount)
            {
                slot.count = newAmount;

                myInventoryUI.UpdateExistingItemSlot(item, newAmount);
                Debug.Log($"[Inventory] {item.ItemName} 수량 {slot.count - amount} → {newAmount}");
                return true;
            }
            else
            {
                Debug.LogWarning($"[Inventory] {item.ItemName} 수량이 최대치({item.MaxAmount})를 초과했습니다.");
                return false;
            }
        }

        // 2. 빈 슬롯(DUMMY)을 찾아 교체
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemData.ItemType == ItemType.DUMMY)
            {
                inventorySlots[i].itemData = item;
                inventorySlots[i].count = amount;

                myInventoryUI.RefreshInventoryUI(); // 전체 UI 갱신
                Debug.Log($"[Inventory] 새 소모품 {item.ItemName} {i}번 슬롯에 등록 ({amount})");
                return true;
            }
        }

        // 3. 인벤토리가 가득 찼을 경우
        Debug.LogWarning("[Inventory] 인벤토리에 빈 공간이 없습니다!");
        return false;
    }

    // 획득한 장비를 비어있는 공간(장비칸 or 인벤토리)에 추가하는 함수
    private bool AddEquipmentItem(EquipmentItemData item)
    {
        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            if (equipmentItemSlot[i].ItemType == ItemType.DUMMY)
            {
                LoadEquipmentItem(item, i);
                myInventoryUI.SetEquippedItemSlotData(item);
                return true;
            }
        }

        return AddItemToInventory(item, 1);
    }

    // 해당 장비 칸에 장비 데이터를 추가하는 메서드
    public void LoadEquipmentItem(EquipmentItemData item, int idx = 0)
    {
        equipmentItemSlot[idx] = item;
        equipmentItemSlot[idx].EquipItem(playerStatus);
    }

    // 장착된 장비를 인벤토리로 옮기는 메서드
    public void UnloadEquipmentItem(int idx = 0)
    {
        EquipmentItemData itemToUnload = equipmentItemSlot[idx];
        if (itemToUnload.ItemType == ItemType.DUMMY) return;

        // 인벤토리에 먼저 추가 시도
        bool addedToInventory = AddItemToInventory(itemToUnload, 1);

        // 인벤토리 추가에 성공한 경우에만 장비 해제
        if (addedToInventory)
        {
            itemToUnload.UnEquipItem(playerStatus);
            equipmentItemSlot[idx] = dummyItemData;
        }
        else
        {
            Debug.LogWarning($"[Inventory] {itemToUnload.ItemName}을(를) 인벤토리로 옮기려 했으나 공간이 부족합니다.");
        }
    }

    // 인벤토리에 아이템 추가 (소모품/장비 공용)
    private bool AddItemToInventory(BasicItemData item, int amount)
    {
        // 빈 슬롯(DUMMY)을 찾아 교체
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemData.ItemType == ItemType.DUMMY)
            {
                inventorySlots[i].itemData = item;
                inventorySlots[i].count = amount;

                myInventoryUI.RefreshInventoryUI(); // 전체 UI 갱신
                Debug.Log($"[Inventory] {item.ItemName} {i}번 슬롯에 신규 등록 ({amount})");
                return true;
            }
        }

        Debug.Log("인벤토리에 빈 공간이 없습니다!");
        return false;
    }

    // 보유중인 아이템 확인용
    public List<BasicItemData> GetOwnedItems()
    {
        List<BasicItemData> result = new List<BasicItemData>();

        result.AddRange(inventorySlots
            .Where(s => s.itemData.ItemType != ItemType.DUMMY)
            .Select(s => s.itemData));

        if (quickSlot != null)
            result.Add(quickSlot);

        foreach (var equip in equipmentItemSlot)
        {
            if (equip != null && equip.ItemType != ItemType.DUMMY)
                result.Add(equip);
        }

        return result;
    }

    // 인벤토리 1번(0-index) 슬롯을 '퀵슬롯'처럼 참조해 UI를 갱신
    public void UpdateQuickSlotReference()
    {
        if (myInventoryUI == null) return;

        // '데이터 슬롯'을 직접 참조
        if (inventorySlots == null || inventorySlots.Count == 0)
        {
            myInventoryUI.QuickSlotImg.DeleteItemData();
            return;
        }

        var firstSlotData = inventorySlots[0]; // 데이터 0번 슬롯
        var data = firstSlotData.itemData;
        var amount = firstSlotData.count;

        if (data == null || data.ItemType != ItemType.CONSUMABLE || amount <= 0)
        {
            myInventoryUI.QuickSlotImg.DeleteItemData();
            return;
        }

        myInventoryUI.SetQuickSLotItemData(data, amount);
    }

    // 인벤토리에 있는 장비와 장착칸의 장비 데이터를 서로 교체하는 함수
    public void SwapEquippedItemWithInventory(int equippedSlotIdx, EquipmentItemData inventoryItemData)
    {
        EquipmentItemData equippedItem = equipmentItemSlot[equippedSlotIdx];

        var slot = inventorySlots.FirstOrDefault(s => s.itemData == inventoryItemData);
        if (slot == null)
        {
            Debug.LogError("[Inventory] 스왑할 아이템을 인벤토리에서 찾을 수 없습니다.");
            return;
        }

        if (equippedItem != null && equippedItem.ItemType != ItemType.DUMMY)
            equippedItem.UnEquipItem(playerStatus);

        equipmentItemSlot[equippedSlotIdx] = inventoryItemData;
        inventoryItemData.EquipItem(playerStatus);

        // 인벤토리 슬롯을 기존 장비로 교체 (더미 처리 포함)
        slot.itemData = equippedItem;
        slot.count = (equippedItem.ItemType == ItemType.DUMMY) ? 0 : 1;

        myInventoryUI.RefreshInventoryUI();
    }

    // 장비 장착칸의 아이템 슬롯끼리 교체하는 함수
    public void SwapEquipmentItemSlots(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= equipmentItemSlot.Length) return;
        if (idx2 < 0 || idx2 >= equipmentItemSlot.Length) return;

        EquipmentItemData temp = equipmentItemSlot[idx1];
        equipmentItemSlot[idx1] = equipmentItemSlot[idx2];
        equipmentItemSlot[idx2] = temp;

        myInventoryUI.RefreshInventoryUI();
    }

    // 인벤토리 데이터 슬롯 2개를 스왑하는 함수
    public void SwapInventorySlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= inventorySlots.Count || indexB < 0 || indexB >= inventorySlots.Count)
        {
            Debug.LogWarning($"[Inventory] 잘못된 스왑 인덱스: {indexA}, {indexB}");
            return;
        }

        // 데이터 리스트(inventorySlots)의 내용을 직접 교환
        InventorySlot temp = inventorySlots[indexA];
        inventorySlots[indexA] = inventorySlots[indexB];
        inventorySlots[indexB] = temp;
    }

    // 인벤토리에 있는 장비를 해당 장비칸에 추가하는 메서드
    public void LoadEquipmentItemFromInventory(EquipmentItemData equipData, int equipSlotIdx)
    {
        equipmentItemSlot[equipSlotIdx] = equipData;
        equipData.EquipItem(playerStatus);

        // 인벤토리에서 해당 아이템 '제거' (더미로 교체)
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == equipData);
        if (slot != null)
        {
            slot.Clear(dummyInventoryItemData);
        }

        myInventoryUI.RefreshInventoryUI();
    }

    // QuickSlotUI.cs의 UpdateQuickSlot에서 호출하는 함수
    public void SetQuickSlot(BasicItemData itemData, int amount)
    {
        quickSlot = itemData as ConsumableItemData;
        quickSlotItemAmount = amount;
    }
}