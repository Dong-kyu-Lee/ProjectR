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

    // 장비 장착 상태 변경 시 발생할 이벤트
    public event Action OnStatusChanged;

    // 인벤토리 관련
    [SerializeField] private int maxInventorySlot = 12;          // 최대 인벤토리 칸 갯수
    [SerializeField] private PlayerStatus playerStatus;           // 플레이어의 Status
    [SerializeField] private List<InventorySlot> inventorySlots;   // 슬롯 기반 인벤토리 구조

    // 장비 관련 칸
    [SerializeField] private int maxEquipSlot = 6;              // 장비칸의 갯수
    [SerializeField] private EquipmentItemData[] equipmentItemSlot; // 장비칸에 장착된 아이템들

    // 그 외
    [SerializeField] private EquipmentItemData dummyItemData;        // 더미데이터. 칸이 비어있음을 나타낼 때 사용함
    [SerializeField] private BasicItemData dummyInventoryItemData; // 인벤토리용 더미
    [SerializeField] private InventoryUI myInventoryUI;            // 인벤토리 UI 내부 필드

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int MaxInventorySlot => maxInventorySlot;
    public int MaxEquipSlot => maxEquipSlot;
    public EquipmentItemData[] EquipmentItemSlot => equipmentItemSlot;

    // 씬 전체 탐색 대신 싱글톤 매니저를 통한 특정 계층 탐색
    public InventoryUI MyInventoryUI
    {
        get
        {
            if (myInventoryUI == null)
            {
                // 1. 매니저와 CharacterInfo가 씬에 존재하는지 확인
                if (InGameUIManager.Instance != null && InGameUIManager.Instance.characterInfoUI != null)
                {
                    // 2. 씬 전체가 아닌 CharacterInfo 하위 계층에서만 InventoryUI를 빠르게 탐색
                    myInventoryUI = InGameUIManager.Instance.characterInfoUI.GetComponentInChildren<InventoryUI>(true);
                }
            }
            return myInventoryUI;
        }
        set => myInventoryUI = value;
    }

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
            return;
        }

        InventorySlot slot = inventorySlots[slotIndex];

        if (slot.itemData != null && slot.itemData.ItemType == ItemType.CONSUMABLE && slot.count > 0)
        {
            ConsumableItemData consumable = slot.itemData as ConsumableItemData;

            if (playerStatus != null)
            {
                consumable.ActivateItemEffect(playerStatus);
            }
            slot.count--;

            if (slot.count <= 0)
            {
                slot.Clear(dummyInventoryItemData);
            }

            if (MyInventoryUI != null)
            {
                MyInventoryUI.RefreshInventoryUI();
            }
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
                return false;
            default:
                return false;
        }

        if (success)
            OnItemAdded?.Invoke(item, amount);

        return success;
    }

    // 아이템 삭제 (버리기 기능용)
    public void RemoveItem(BasicItemData item)
    {
        if (item == null) return;
        Debug.Log("제거 시작");
        bool isRemoved = false;

        // 인벤토리 슬롯에서 검색
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemData == item)
            {
                inventorySlots[i].Clear(dummyInventoryItemData);
                isRemoved = true;
                break;
            }
        }

        // 장비 슬롯에서 검색
        if (!isRemoved)
        {
            for (int i = 0; i < equipmentItemSlot.Length; i++)
            {
                if (equipmentItemSlot[i] == item)
                {
                    // 장착 중인 아이템이므로 스탯 효과 제거(UnEquip) 먼저 실행
                    if (playerStatus != null)
                    {
                        equipmentItemSlot[i].UnEquipItem(playerStatus);
                    }

                    equipmentItemSlot[i] = dummyItemData;

                    isRemoved = true;
                    break;
                }
            }
        }

        // 삭제된 내역이 있다면 UI 전체 갱신
        if (isRemoved)
        {
            UpdateQuickSlotReference();

            if (MyInventoryUI != null)
            {
                MyInventoryUI.RefreshInventoryUI();
            }

            // 스탯 갱신 알림
            OnStatusChanged?.Invoke();
        }
    }

    // 획득한 소모품 아이템 데이터를 인벤토리에 추가하는 메서드
    private bool AddConsumableItem(ConsumableItemData item, int amount)
    {
        int remainingAmount = amount;

        // 1. 이미 존재하는 동일 아이템 슬롯들에 먼저 채워넣기
        foreach (var slot in inventorySlots)
        {
            if (slot.itemData == item && slot.count < item.MaxAmount)
            {
                int space = item.MaxAmount - slot.count; // 슬롯에 남은 빈자리
                if (remainingAmount <= space)
                {
                    slot.count += remainingAmount;
                    if (MyInventoryUI != null) MyInventoryUI.UpdateExistingItemSlot(item, slot.count);
                    return true; // 다 채워넣었으므로 종료
                }
                else
                {
                    slot.count = item.MaxAmount; // 이 슬롯은 꽉 채움
                    remainingAmount -= space;    // 남은 수량 갱신
                    if (MyInventoryUI != null) MyInventoryUI.UpdateExistingItemSlot(item, slot.count);
                }
            }
        }

        // 2. 남은 수량이 있다면 빈 슬롯(DUMMY)을 찾아 교체
        if (remainingAmount > 0)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].itemData.ItemType == ItemType.DUMMY)
                {
                    inventorySlots[i].itemData = item;
                    inventorySlots[i].count = remainingAmount;

                    if (MyInventoryUI != null) MyInventoryUI.RefreshInventoryUI();
                    return true;
                }
            }
        }

        // 인벤토리가 완전히 가득 찼을 경우
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

                if (MyInventoryUI != null)
                {
                    MyInventoryUI.SetEquippedItemSlotData(item);
                }
                return true;
            }
        }
        return AddItemToInventory(item, 1);
    }

    // 해당 장비 칸에 장비 데이터를 추가하는 메서드
    public void LoadEquipmentItem(EquipmentItemData item, int idx = 0)
    {
        equipmentItemSlot[idx] = item;

        if (playerStatus != null)
        {
            equipmentItemSlot[idx].EquipItem(playerStatus);   // 장비 장착 효과 적용
        }

        // 장비 장착으로 스탯이 변했음을 알림
        OnStatusChanged?.Invoke();
    }

    private bool AddItemToInventory_NoRefresh(BasicItemData item, int amount)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemData.ItemType == ItemType.DUMMY)
            {
                inventorySlots[i].itemData = item;
                inventorySlots[i].count = amount;
                return true;
            }
        }
        return false;
    }

    // 인벤토리에 아이템 추가 (소모품/장비 공용)
    private bool AddItemToInventory(BasicItemData item, int amount)
    {
        if (AddItemToInventory_NoRefresh(item, amount))
        {
            if (MyInventoryUI != null)
            {
                MyInventoryUI.RefreshInventoryUI();
            }
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
            return false;
        }
        EquipmentItemData itemToUnload = equipmentItemSlot[equipSlotIdx];
        if (itemToUnload.ItemType == ItemType.DUMMY) return false;

        // 드롭한 칸(invSlotIdx)이 비어있는지 확인
        if (inventorySlots[invSlotIdx].itemData.ItemType == ItemType.DUMMY)
        {
            // 데이터 이동
            inventorySlots[invSlotIdx].itemData = itemToUnload;
            inventorySlots[invSlotIdx].count = 1;

            // 장비칸 비우기
            if (playerStatus != null)
            {
                itemToUnload.UnEquipItem(playerStatus);
            }
            equipmentItemSlot[equipSlotIdx] = dummyItemData;

            OnStatusChanged?.Invoke();

            return true;
        }
        else
        {
            return false;
        }
    }

    // 장착된 장비를 인벤토리로 옮기는 메서드
    public void UnloadEquipmentItem(int idx = 0)
    {
        EquipmentItemData itemToUnload = equipmentItemSlot[idx];
        if (itemToUnload.ItemType == ItemType.DUMMY) return;

        bool addedToInventory = AddItemToInventory_NoRefresh(itemToUnload, 1); // 첫 빈칸에 추가

        if (addedToInventory)
        {
            if (playerStatus != null)
            {
                itemToUnload.UnEquipItem(playerStatus);   // 장비 장착 효과 해제
            }
            equipmentItemSlot[idx] = dummyItemData;

            if (MyInventoryUI != null)
            {
                MyInventoryUI.RefreshInventoryUI();
            }

            // 장비 해제로 스탯이 변했음을 알림
            OnStatusChanged?.Invoke();
        }
        else
        {
            return;
        }
    }

    // 보유중인 아이템 확인용
    public List<BasicItemData> GetOwnedItems()
    {
        List<BasicItemData> result = new List<BasicItemData>();

        // 인벤토리 아이템
        result.AddRange(inventorySlots
            .Where(s => s.itemData.ItemType != ItemType.DUMMY)
            .Select(s => s.itemData));

        // 장비 슬롯 아이템
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
        if (MyInventoryUI == null) return;
        if (inventorySlots == null || inventorySlots.Count == 0)
        {
            if (MyInventoryUI.QuickSlotImg != null)
            {
                MyInventoryUI.QuickSlotImg.DeleteItemData();
            }
            return;
        }
        var firstSlotData = inventorySlots[0];
        var data = firstSlotData.itemData;
        var amount = firstSlotData.count;

        if (data == null || data.ItemType != ItemType.CONSUMABLE || amount <= 0)
        {
            if (MyInventoryUI.QuickSlotImg != null)
            {
                MyInventoryUI.QuickSlotImg.DeleteItemData();
            }
            return;
        }
        MyInventoryUI.SetQuickSLotItemData(data, amount);
    }

    // 인벤토리에 있는 장비와 장착칸의 장비 데이터를 서로 교체하는 함수
    public void SwapEquippedItemWithInventory(int equippedSlotIdx, int inventorySlotIdx)
    {
        // 현재 장비칸에 장착된 아이템 임시 저장
        EquipmentItemData equippedItem = equipmentItemSlot[equippedSlotIdx];

        // 인덱스를 사용하여 정확한 인벤토리 슬롯을 즉시 참조
        InventorySlot invSlot = inventorySlots[inventorySlotIdx];
        EquipmentItemData inventoryItemData = invSlot.itemData as EquipmentItemData;

        // 기존 장비 해제
        if (equippedItem != null && equippedItem.ItemType != ItemType.DUMMY && playerStatus != null)
            equippedItem.UnEquipItem(playerStatus);

        // [방어 코드 적용] 새로운 장비 장착 (null이면 dummyItemData 삽입)
        equipmentItemSlot[equippedSlotIdx] = inventoryItemData ?? dummyItemData;
        if (inventoryItemData != null && playerStatus != null)
        {
            inventoryItemData.EquipItem(playerStatus);
        }

        // 인벤토리 슬롯을 기존 장비로 교체
        invSlot.itemData = equippedItem;
        invSlot.count = (equippedItem.ItemType == ItemType.DUMMY) ? 0 : 1;

        if (MyInventoryUI != null)
        {
            MyInventoryUI.RefreshInventoryUI();
        }

        // 장비 교체로 스탯이 변했음을 알림
        OnStatusChanged?.Invoke();
    }

    // 장비 장착칸의 아이템 슬롯끼리 교체하는 함수 (순서 변경용)
    public void SwapEquipmentItemSlots(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= equipmentItemSlot.Length) return;
        if (idx2 < 0 || idx2 >= equipmentItemSlot.Length) return;
        EquipmentItemData temp = equipmentItemSlot[idx1];
        equipmentItemSlot[idx1] = equipmentItemSlot[idx2];
        equipmentItemSlot[idx2] = temp;

        if (MyInventoryUI != null)
        {
            MyInventoryUI.RefreshInventoryUI();
        }
    }

    // 인벤토리 데이터 슬롯 2개를 스왑하는 함수
    public void SwapInventorySlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= inventorySlots.Count || indexB < 0 || indexB >= inventorySlots.Count)
        {
            return;
        }
        InventorySlot temp = inventorySlots[indexA];
        inventorySlots[indexA] = inventorySlots[indexB];
        inventorySlots[indexB] = temp;
    }

    // 인벤토리 -> 장비칸 (드래그용, Refresh 없음)
    public void LoadEqFromInv_NoRefresh(int inventorySlotIdx, int equipSlotIdx)
    {
        if (inventorySlotIdx < 0 || inventorySlotIdx >= inventorySlots.Count ||
            equipSlotIdx < 0 || equipSlotIdx >= equipmentItemSlot.Length)
        {
            return;
        }

        InventorySlot invSlot = inventorySlots[inventorySlotIdx];
        EquipmentItemData equipData = invSlot.itemData as EquipmentItemData;

        // [방어 코드 적용] 장비 장착 (null이면 dummyItemData 삽입)
        equipmentItemSlot[equipSlotIdx] = equipData ?? dummyItemData;
        if (equipData != null && playerStatus != null)
        {
            equipData.EquipItem(playerStatus);
        }

        // 인벤토리에서 해당 아이템 제거
        invSlot.Clear(dummyInventoryItemData);
    }

    // 인벤토리에 있는 장비를 해당 장비칸에 추가하는 메서드
    public void LoadEquipmentItemFromInventory(int inventorySlotIdx, int equipSlotIdx)
    {
        LoadEqFromInv_NoRefresh(inventorySlotIdx, equipSlotIdx);

        if (MyInventoryUI != null)
        {
            MyInventoryUI.RefreshInventoryUI();
        }

        // 드래그를 통해 장비를 장착했으므로 스탯 갱신 알림
        OnStatusChanged?.Invoke();
    }
    // 연습장에서 획득한 투척 아이템을 모두 제거하는 함수 (던전 진입 전 호출용)
    public void ClearAllThrowableItems()
    {
        bool isRemoved = false;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];

            // 데이터가 있고, 소모품이며, 투척류인 경우
            if (slot.itemData != null && slot.itemData.ItemType == ItemType.CONSUMABLE)
            {
                ConsumableItemData consumable = slot.itemData as ConsumableItemData;
                if (consumable != null && consumable.kind == ConsumableKind.Throwable)
                {
                    // 슬롯 비우기
                    slot.Clear(dummyInventoryItemData);
                    isRemoved = true;
                }
            }
        }

        // 삭제된 내역이 있다면 UI 전체 갱신
        if (isRemoved)
        {
            UpdateQuickSlotReference();

            if (MyInventoryUI != null)
            {
                MyInventoryUI.RefreshInventoryUI();
            }

            Debug.Log("[Inventory] 던전 진입을 위해 투척 아이템이 모두 초기화되었습니다.");
        }
    }
}