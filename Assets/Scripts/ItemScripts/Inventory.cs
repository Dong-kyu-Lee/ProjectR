using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static event Action<BasicItemData, int> OnItemAdded;

    //인벤토리 관련
    [SerializeField]
    private int maxInventorySlot = 10;                  //최대 인벤토리 칸 갯수
    [SerializeField]
    private PlayerStatus playerStatus;                  //플레이어의 Status
    [SerializeField]
    private Dictionary<string, (BasicItemData Data, int Count)> inventory;   //아이템 이름 기준 딕셔너리 구조

    //소모품 관련 칸
    private ConsumableItemData quickSlot = null;        //퀵슬롯 아이템 데이터
    private int quickSlotItemAmount = 0;                //퀵슬롯에 있는 아이템 갯수

    //장비 관련 칸
    [SerializeField] private int maxEquipSlot = 6;      //장비칸의 갯수
    [SerializeField] private EquipmentItemData[] equipmentItemSlot; // 장비칸에 장착된 아이템들

    //그 외
    [SerializeField] private EquipmentItemData dummyItemData;   //더미데이터. 칸이 비어있음을 나타날 때 사용함.
    [SerializeField] private InventoryUI myInventoryUI;         //인벤토리 UI. UI의 초기화함수 Init()이 호출될 때 참조가 연결됨.

    public int QuickSlotItemAmount
    {
        get { return quickSlotItemAmount; }
        set
        {
            quickSlotItemAmount = value;
            if (quickSlotItemAmount < 0) quickSlotItemAmount = 0;
        }
    }
    public ConsumableItemData QuickSlot { get { return quickSlot; } }
    public int QuickSlotAmount { get { return quickSlotItemAmount; } }
    public Dictionary<string, (BasicItemData Data, int Count)> InventoryDict { get { return inventory; } }
    public int MaxInventorySlot { get { return maxInventorySlot; } }
    public int MaxEquipSlot { get { return maxEquipSlot; } }
    public EquipmentItemData[] EquipmentItemSlot { get { return equipmentItemSlot; } }
    public InventoryUI MyInventoryUI { get { return myInventoryUI; } set { myInventoryUI = value; } }

    private void Awake()
    {
        inventory = new Dictionary<string, (BasicItemData, int)>();
        playerStatus = GetComponentInParent<PlayerStatus>();
        equipmentItemSlot = new EquipmentItemData[maxEquipSlot];
        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            equipmentItemSlot[i] = dummyItemData;
        }
    }

    //퀵슬롯에 있는 아이템을 사용하는 메서드.
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

    //인벤토리에 있는 아이템 데이터를 퀵슬롯에 로드하는 메서드
    public void LoadToQuickSlotFromInventory(BasicItemData item)
    {
        string key = item.ItemName;
        if (!inventory.ContainsKey(key)) return;

        quickSlot = item as ConsumableItemData;
        quickSlotItemAmount = inventory[key].Count;
        inventory.Remove(key);
    }

    //아이템을 획득을 처리하는 메서드. 아이템의 종류에 따라 다른 함수 실행
    public void AddItem(BasicItemData item, int amount = 1)
    {
        switch (item.ItemType)
        {
            case ItemType.CONSUMABLE:
                AddConsumableItem(item as ConsumableItemData, amount);
                break;
            case ItemType.EQUIPMENT:
                AddEquipmentItem(item as EquipmentItemData);
                break;
            case ItemType.DUMMY:
                Debug.LogWarning($"[Inventory] DUMMY 아이템은 추가하지 않습니다: {item.ItemName}");
                break;
            default:
                Debug.LogWarning($"[Inventory] 정의되지 않은 아이템 타입: {item.ItemType}");
                break;
        }

        OnItemAdded?.Invoke(item, amount);
    }

    //획득한 소모품 아이템 데이터를 퀵슬롯 or 인벤토리에 추가하는 메서드
    private bool AddConsumableItem(ConsumableItemData item, int amount)
    {
        return false; // 수정 예정
    }

    //획득한 장비를 비어있는 공간(장비칸 or 인벤토리)에 추가하는 함수
    private bool AddEquipmentItem(EquipmentItemData item)
    {
        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            if (equipmentItemSlot[i].ItemType == ItemType.DUMMY)
            {
                LoadEquipmentItem(item, i);
                return true;
            }
        }

        return AddItemsToInventoryWithUiUpdate(item, 1);
    }

    //해당 장비 칸에 장비 데이터를 추가하는 메서드
    public void LoadEquipmentItem(EquipmentItemData item, int idx = 0)
    {
        equipmentItemSlot[idx] = item;
        equipmentItemSlot[idx].EquipItem(playerStatus);     //장비 장착 효과 적용
    }

    //인벤토리에 있는 장비를 해당 장비칸에 추가하는 메서드
    public void LoadEquipmentItemFromInventory(EquipmentItemData equipData, int equipSlotIdx)
    {
        LoadEquipmentItem(equipData, equipSlotIdx);
        inventory.Remove(equipData.ItemName);
    }

    //대상 장비 장착칸에 있는 장비 데이터를 인벤토리로 옮기는 메서드
    public void UnloadEquipmentItem(int idx = 0)
    {
        equipmentItemSlot[idx].UnEquipItem(playerStatus);   //장비 장착 효과 해제
        AddItemsToInventory(equipmentItemSlot[idx], 1);
        equipmentItemSlot[idx] = dummyItemData;
    }

    //인벤토리에 있는 장비와 장착칸의 장비 데이터를 서로 교체하는 함수
    public void SwapEquippedItemWithInventory(int equippedSlotIdx, EquipmentItemData inventoryItemData)
    {
        UnloadEquipmentItem(equippedSlotIdx);
        LoadEquipmentItemFromInventory(inventoryItemData, equippedSlotIdx);
    }

    //장비 장착칸의 아이템 슬롯끼리 교체하는 함수. 순서만 바꾸는 용도.
    public void SwapEquipmentItemSlots(int idx1, int idx2)
    {
        EquipmentItemData temp = equipmentItemSlot[idx1];
        equipmentItemSlot[idx1] = equipmentItemSlot[idx2];
        equipmentItemSlot[idx2] = temp;
    }

    //UI도 업데이트 하는 함수
    private bool AddItemsToInventoryWithUiUpdate(BasicItemData item, int amount)
    {
        string key = item.ItemName;

        if (inventory.ContainsKey(key))
        {
            var current = inventory[key];
            int newAmount = current.Count + amount;

            if (newAmount <= item.MaxAmount)
            {
                inventory[key] = (current.Data, newAmount);
                myInventoryUI.UpdateItemSlotAmount(current.Data, newAmount);
                return true;
            }
            else
            {
                Debug.Log("아이템이 최대 갯수를 초과했습니다!");
                return false;
            }
        }
        else if (inventory.Count < maxInventorySlot)
        {
            inventory.Add(key, (item, amount));
            myInventoryUI.SetInventorySlotData(item, amount);
            return true;
        }
        else
        {
            Debug.Log("인벤토리에 빈 공간이 없습니다!");
            return false;
        }
    }

    private bool AddItemsToInventory(BasicItemData item, int amount)
    {
        string key = item.ItemName;

        // 소비 아이템일 때만 수량 합치기
        if (item.ItemType == ItemType.CONSUMABLE)
        {
            if (inventory.ContainsKey(key))
            {
                var current = inventory[key];
                int newAmount = current.Count + amount;

                if (newAmount <= item.MaxAmount)
                {
                    inventory[key] = (current.Data, newAmount);
                    myInventoryUI.UpdateItemSlotAmount(current.Data, newAmount);
                    Debug.Log($"[Inventory] {key} 수량 {current.Count} → {newAmount}");
                    return true;
                }
                else
                {
                    Debug.Log("아이템이 최대 갯수를 초과했습니다!");
                    return false;
                }
            }
        }

        // 장비 또는 신규 아이템 추가
        if (inventory.Count < maxInventorySlot)
        {
            inventory[key] = (item, amount);
            myInventoryUI.SetInventorySlotData(item, amount);
            Debug.Log($"[Inventory] {key} 신규 등록 ({amount})");
            return true;
        }

        Debug.Log("인벤토리에 빈 공간이 없습니다!");
        return false;
    }


    //보유중인 아이템 확인용
    public List<BasicItemData> GetOwnedItems()
    {
        List<BasicItemData> result = new List<BasicItemData>();

        // 1. 인벤토리 아이템
        result.AddRange(inventory.Values.Select(v => v.Data));

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

        if (data != null && data.ItemType == ItemType.CONSUMABLE && amount > 0)
        {
            myInventoryUI.SetQuickSLotItemData(data, amount);
        }
        else
        {
            myInventoryUI.QuickSlotImg.DeleteItemData();
        }
    }
}
