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
    [SerializeField] private int maxInventorySlot = 12;          // 최대 인벤토리 칸 갯수
    [SerializeField] private PlayerStatus playerStatus;           // 플레이어의 Status
    [SerializeField] private List<InventorySlot> inventorySlots;   // 슬롯 기반 인벤토리 구조

    // 장비 관련 칸
    [SerializeField] private int maxEquipSlot = 6;             // 장비칸의 갯수
    [SerializeField] private EquipmentItemData[] equipmentItemSlot; // 장비칸에 장착된 아이템들

    // 그 외
    [SerializeField] private EquipmentItemData dummyItemData;        // 더미데이터. 칸이 비어있음을 나타낼 때 사용함
    [SerializeField] private BasicItemData dummyInventoryItemData; // (신규) 인벤토리용 더미
    [SerializeField] private InventoryUI myInventoryUI;            // 인벤토리 UI. Init()이 호출될 때 참조 연결됨.

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int MaxInventorySlot => maxInventorySlot;
    public int MaxEquipSlot => maxEquipSlot;
    public EquipmentItemData[] EquipmentItemSlot => equipmentItemSlot;
    public InventoryUI MyInventoryUI { get => myInventoryUI; set => myInventoryUI = value; }

    private void Awake()
    {
        inventorySlots = new List<InventorySlot>(maxInventorySlot);
        playerStatus = GetComponentInParent<PlayerStatus>();

        // (고정 크기 리스트) 더미 슬롯으로 미리 채움
        for (int i = 0; i < maxInventorySlot; i++)
        {
            inventorySlots.Add(new InventorySlot(dummyInventoryItemData, 0));
        }

        equipmentItemSlot = new EquipmentItemData[maxEquipSlot];
        for (int i = 0; i < equipmentItemSlot.Length; i++)
            equipmentItemSlot[i] = dummyItemData;
    }

    // 퀵슬롯에 있는 아이템을 사용하는 메서드. (0번 슬롯 사용)
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
            slot.count--;

            if (slot.count <= 0)
            {
                slot.Clear(dummyInventoryItemData);
            }

            myInventoryUI.RefreshInventoryUI();
        }
        else
        {
            Debug.Log($"{slotIndex}번 슬롯에 사용할 아이템이 없습니다.");
        }
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

                myInventoryUI.RefreshInventoryUI();
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
        equipmentItemSlot[idx].EquipItem(playerStatus);   // 장비 장착 효과 적용
    }

    // 인벤토리에 아이템 추가 (NoRefresh)
    private bool AddItemToInventory_NoRefresh(BasicItemData item, int amount)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemData.ItemType == ItemType.DUMMY)
            {
                inventorySlots[i].itemData = item;
                inventorySlots[i].count = amount;
                Debug.Log($"[Inventory] {item.ItemName} {i}번 슬롯에 신규 등록 ({amount})");
                return true;
            }
        }
        Debug.Log("인벤토리에 빈 공간이 없습니다!");
        return false;
    }

    // 인벤토리에 아이템 추가 (소모품/장비 공용)
    private bool AddItemToInventory(BasicItemData item, int amount)
    {
        if (AddItemToInventory_NoRefresh(item, amount))
        {
            myInventoryUI.RefreshInventoryUI();
            return true;
        }
        return false;
    }

    // 장비칸 -> 인벤토리 (드래그용, Refresh 없음, 지정 슬롯)
    public bool UnloadEqToInv_NoRefresh(int equipSlotIdx, int invSlotIdx)
    {
        if (equipSlotIdx < 0 || equipSlotIdx >= equipmentItemSlot.Length ||
            invSlotIdx < 0 || invSlotIdx >= inventorySlots.Count)
        {
            Debug.LogWarning("[Inventory] UnloadEqToInv_NoRefresh: 잘못된 인덱스입니다.");
            return false;
        }
        EquipmentItemData itemToUnload = equipmentItemSlot[equipSlotIdx];
        if (itemToUnload.ItemType == ItemType.DUMMY) return false;

        if (inventorySlots[invSlotIdx].itemData.ItemType == ItemType.DUMMY)
        {
            inventorySlots[invSlotIdx].itemData = itemToUnload;
            inventorySlots[invSlotIdx].count = 1;
            itemToUnload.UnEquipItem(playerStatus);
            equipmentItemSlot[equipSlotIdx] = dummyItemData;
            return true;
        }
        else
        {
            Debug.LogWarning($"[Inventory] 타겟 슬롯 {invSlotIdx}이 비어있지 않습니다.");
            return false;
        }
    }

    // 장착된 장비를 인벤토리로 옮기는 메서드
    public void UnloadEquipmentItem(int idx = 0)
    {
        EquipmentItemData itemToUnload = equipmentItemSlot[idx];
        if (itemToUnload.ItemType == ItemType.DUMMY) return;

        bool addedToInventory = AddItemToInventory_NoRefresh(itemToUnload, 1);

        if (addedToInventory)
        {
            itemToUnload.UnEquipItem(playerStatus);   // 장비 장착 효과 해제
            equipmentItemSlot[idx] = dummyItemData;
            myInventoryUI.RefreshInventoryUI();
        }
        else
        {
            Debug.LogWarning($"[Inventory] {itemToUnload.ItemName}을(를) 인벤토리로 옮기려 했으나 공간이 부족합니다.");
        }
    }

    // 보유중인 아이템 확인용
    public List<BasicItemData> GetOwnedItems()
    {
        List<BasicItemData> result = new List<BasicItemData>();

        // 1. 인벤토리 아이템 (더미 제외)
        result.AddRange(inventorySlots
            .Where(s => s.itemData.ItemType != ItemType.DUMMY)
            .Select(s => s.itemData));

        // 2. 장비 슬롯 아이템 (더미 제외)
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
        if (inventorySlots == null || inventorySlots.Count == 0)
        {
            myInventoryUI.QuickSlotImg.DeleteItemData();
            return;
        }
        var firstSlotData = inventorySlots[0];
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
        // 1. 현재 장비칸에 장착된 아이템 임시 저장
        EquipmentItemData equippedItem = equipmentItemSlot[equippedSlotIdx];

        // 2. 인벤토리에서 교체할 아이템 슬롯 찾기
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == inventoryItemData);
        if (slot == null)
        {
            Debug.LogError("[Inventory] 스왑할 아이템을 인벤토리에서 찾을 수 없습니다.");
            return;
        }

        // 3. 기존 장비 해제
        if (equippedItem != null && equippedItem.ItemType != ItemType.DUMMY)
            equippedItem.UnEquipItem(playerStatus);

        // 4. 새로운 장비 장착
        equipmentItemSlot[equippedSlotIdx] = inventoryItemData;
        inventoryItemData.EquipItem(playerStatus);

        // 5. 인벤토리 슬롯을 기존 장비로 교체
        slot.itemData = equippedItem;
        slot.count = (equippedItem.ItemType == ItemType.DUMMY) ? 0 : 1;

        myInventoryUI.RefreshInventoryUI();
    }

    // 장비 장착칸의 아이템 슬롯끼리 교체하는 함수 (순서 변경용)
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
        InventorySlot temp = inventorySlots[indexA];
        inventorySlots[indexA] = inventorySlots[indexB];
        inventorySlots[indexB] = temp;
    }

    // 인벤토리 -> 장비칸 (드래그용, Refresh 없음)
    public void LoadEqFromInv_NoRefresh(EquipmentItemData equipData, int equipSlotIdx)
    {
        if (equipData == null || equipSlotIdx < 0 || equipSlotIdx >= equipmentItemSlot.Length)
        {
            Debug.LogWarning("[Inventory] LoadEqFromInv_NoRefresh: 잘못된 데이터 또는 인덱스입니다.");
            return;
        }
        // 1. 장비 장착
        equipmentItemSlot[equipSlotIdx] = equipData;
        equipData.EquipItem(playerStatus);

        // 2. 인벤토리에서 해당 아이템 제거
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == equipData);
        if (slot != null)
        {
            slot.Clear(dummyInventoryItemData);
        }
        else
        {
            Debug.LogWarning($"[Inventory] 인벤토리에서 {equipData.ItemName}을(를) 찾지 못해 제거하지 못했습니다.");
        }
    }

    // 인벤토리에 있는 장비를 해당 장비칸에 추가하는 메서드
    public void LoadEquipmentItemFromInventory(EquipmentItemData equipData, int equipSlotIdx)
    {
        LoadEqFromInv_NoRefresh(equipData, equipSlotIdx);
        myInventoryUI.RefreshInventoryUI();
    }
}