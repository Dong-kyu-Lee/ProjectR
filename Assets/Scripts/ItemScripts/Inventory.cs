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
}

public class Inventory : MonoBehaviour
{
    public static event Action<BasicItemData, int> OnItemAdded;

    // 인벤토리 관련
    [SerializeField] private int maxInventorySlot = 12;               // 최대 인벤토리 칸 갯수
    [SerializeField] private PlayerStatus playerStatus;               // 플레이어의 Status
    [SerializeField] private List<InventorySlot> inventorySlots;      // 슬롯 기반 인벤토리 구조

    // 소모품 관련 칸
    private ConsumableItemData quickSlot = null;                      // 퀵슬롯 아이템 데이터
    private int quickSlotItemAmount = 0;                              // 퀵슬롯에 있는 아이템 갯수

    // 장비 관련 칸
    [SerializeField] private int maxEquipSlot = 6;                    // 장비칸의 갯수
    [SerializeField] private EquipmentItemData[] equipmentItemSlot;   // 장비칸에 장착된 아이템들

    // 그 외
    [SerializeField] private EquipmentItemData dummyItemData;         // 더미데이터. 칸이 비어있음을 나타낼 때 사용함
    [SerializeField] private InventoryUI myInventoryUI;               // 인벤토리 UI. Init()이 호출될 때 참조 연결됨.

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
        inventorySlots = new List<InventorySlot>();
        playerStatus = GetComponentInParent<PlayerStatus>();

        equipmentItemSlot = new EquipmentItemData[maxEquipSlot];
        for (int i = 0; i < equipmentItemSlot.Length; i++)
            equipmentItemSlot[i] = dummyItemData;
    }

    // 퀵슬롯에 있는 아이템을 사용하는 메서드.
    public void UseQuickSlotItem()
    {
        if (quickSlot)
        {
            quickSlot.ActivateItemEffect(playerStatus);
            QuickSlotItemAmount--;
            myInventoryUI.QuickSlotImg.SetItemAmountData(quickSlotItemAmount);

            if (quickSlotItemAmount <= 0)
            {
                quickSlot = null;
                quickSlotItemAmount = 0;
                myInventoryUI.QuickSlotImg.DeleteItemData();
            }
        }
        else
        {
            Debug.Log("로드된 아이템이 없습니다.");
        }
    }

    // 인벤토리에 있는 아이템 데이터를 퀵슬롯에 로드하는 메서드
    public void LoadToQuickSlotFromInventory(BasicItemData item)
    {
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == item);
        if (slot == null) return;

        quickSlot = item as ConsumableItemData;
        quickSlotItemAmount = slot.count;
        inventorySlots.Remove(slot);
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
                myInventoryUI.UpdateItemSlotAmount(item, newAmount);
                Debug.Log($"[Inventory] {item.ItemName} 수량 {slot.count - amount} → {newAmount}");
                return true;
            }
            else
            {
                Debug.LogWarning($"[Inventory] {item.ItemName} 수량이 최대치({item.MaxAmount})를 초과했습니다.");
                return false;
            }
        }

        // 2. 인벤토리에 없고, 빈 칸이 있을 경우 새로 추가
        if (inventorySlots.Count < maxInventorySlot)
        {
            inventorySlots.Add(new InventorySlot(item, amount));
            myInventoryUI.SetInventorySlotData(item, amount);
            Debug.Log($"[Inventory] 새 소모품 {item.ItemName} 등록 ({amount})");
            return true;
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
        equipmentItemSlot[idx].EquipItem(playerStatus);     // 장비 장착 효과 적용
    }

    // 장착된 장비를 인벤토리로 옮기는 메서드
    public void UnloadEquipmentItem(int idx = 0)
    {
        equipmentItemSlot[idx].UnEquipItem(playerStatus);   // 장비 장착 효과 해제
        AddItemToInventory(equipmentItemSlot[idx], 1);
        equipmentItemSlot[idx] = dummyItemData;
    }

    // 인벤토리에 아이템 추가 (소모품/장비 공용)
    private bool AddItemToInventory(BasicItemData item, int amount)
    {
        if (inventorySlots.Count >= maxInventorySlot)
        {
            Debug.Log("인벤토리에 빈 공간이 없습니다!");
            return false;
        }

        inventorySlots.Add(new InventorySlot(item, amount));
        myInventoryUI.SetInventorySlotData(item, amount);
        Debug.Log($"[Inventory] {item.ItemName} 신규 등록 ({amount})");
        return true;
    }

    // 보유중인 아이템 확인용
    public List<BasicItemData> GetOwnedItems()
    {
        List<BasicItemData> result = new List<BasicItemData>();

        // 1. 인벤토리 아이템
        result.AddRange(inventorySlots.Select(s => s.itemData));

        // 2. 퀵슬롯 아이템
        if (quickSlot != null)
            result.Add(quickSlot);

        // 3. 장비 슬롯 아이템
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

        var firstSlot = myInventoryUI.GetInventorySlotUI(0);
        if (firstSlot == null)
        {
            myInventoryUI.QuickSlotImg.DeleteItemData();
            return;
        }

        var data = firstSlot.NowItemData;
        var amount = firstSlot.ItemCount;

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

        // 2. 기존 장비 해제
        if (equippedItem != null && equippedItem.ItemType != ItemType.DUMMY)
            equippedItem.UnEquipItem(playerStatus);

        // 3. 새로운 장비 장착
        equipmentItemSlot[equippedSlotIdx] = inventoryItemData;
        inventoryItemData.EquipItem(playerStatus);

        // 4. 인벤토리에서 기존 장비로 교체
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == inventoryItemData);
        if (slot != null)
        {
            slot.itemData = equippedItem;  // 교체
            slot.count = 1;
        }

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

    // 인벤토리에 있는 장비를 해당 장비칸에 추가하는 메서드
    public void LoadEquipmentItemFromInventory(EquipmentItemData equipData, int equipSlotIdx)
    {
        // 1. 장비 장착
        equipmentItemSlot[equipSlotIdx] = equipData;
        equipData.EquipItem(playerStatus);

        // 2. 인벤토리에서 해당 아이템 제거
        var slot = inventorySlots.FirstOrDefault(s => s.itemData == equipData);
        if (slot != null)
            inventorySlots.Remove(slot);

        myInventoryUI.RefreshInventoryUI();
    }

}
