using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //인벤토리 관련
    [SerializeField]
    private int maxInventorySlot = 10;
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    private Dictionary<BasicItemData, int> inventory;  //인벤토리 딕셔너리<아이템정보, 갯수>

    //소모품 관련 칸
    private ConsumableItemData quickSlot = null;
    private int quickSlotItemAmount = 0;

    //장비 관련 칸
    [SerializeField] private int maxEquipSlot = 6;
    [SerializeField] private EquipmentItemData[] equipmentItemSlot;

    //그 외
    [SerializeField] private EquipmentItemData dummyItemData; //더미데이터
    [SerializeField] private InventoryUI myInventoryUI;

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
    public Dictionary<BasicItemData, int> InventoryDict { get { return inventory; } }
    public int MaxInventorySlot { get { return maxInventorySlot; } }
    public int MaxEquipSlot { get { return maxEquipSlot; } }
    public EquipmentItemData[] EquipmentItemSlot { get { return equipmentItemSlot; } }
    public InventoryUI MyInventoryUI { get { return myInventoryUI; } set { myInventoryUI = value; } }

    private void Awake()
    {
        inventory = new Dictionary<BasicItemData, int>();
        playerStatus = GetComponentInParent<PlayerStatus>();
        equipmentItemSlot = new EquipmentItemData[maxEquipSlot];
        for (int i = 0; i < maxEquipSlot; i++)
        {
            equipmentItemSlot[i] = dummyItemData;
        }
    }

    //퀵슬롯에 있는 아이템을 사용하는 메서드. 디버그용
    public void UseQuickSlotItem()
    {
        if (quickSlot)
        {
            quickSlot.ActivateItemEffect(playerStatus);
            QuickSlotItemAmount--;

            if (quickSlotItemAmount <= 0)
            {
                quickSlot = null;
                quickSlotItemAmount = 0;
            }
        }
        else
        {
            Debug.Log("로드된 아이템이 없습니다.");
        }
    }

    //아이템을 퀵슬롯에 로드하는 메서드
    public void LoadToQuickSlot(BasicItemData item, int amount = 1)
    {
        quickSlot = item as ConsumableItemData;
        quickSlotItemAmount += amount;
    }

    //아이템을 인벤토리에 추가하는 메서드 
    public bool AddItem(BasicItemData item, int amount = 1)
    {
        switch (item.ItemType)
        {
            case ItemType.CONSUMABLE:
                return AddConsumableItem(item as ConsumableItemData, amount);
            case ItemType.EQUIPMENT:
                return AddEquipmentItem(item as EquipmentItemData);
            default:
                Debug.Log("아이템이 어떠한 클래스 타입이랑도 매칭되지 않음");
                return false;
        }
    }

    private bool AddConsumableItem(ConsumableItemData item, int amount)
    {
        /*if (!quickSlot || quickSlot == item) //퀵슬롯에 아이템이 없거나 같은 아이템이 로드된 경우. 아이템 디버그용임.
        {
            LoadToQuickSlot(item, amount);
            return true;
        }*/

        return AddItemsToInventoryWithUiUpdate(item, amount);
    }

    //장비를 비어있는 공간(장비칸, 인벤토리)에 추가하는 함수
    private bool AddEquipmentItem(EquipmentItemData item)
    {
        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            if (equipmentItemSlot[i].ItemType == ItemType.DUMMY)
            {
                LoadEquipmentItem(item, i);
                MyInventoryUI.SetItemToUI(item, 1);
                return true;
            }
        }

        return AddItemsToInventoryWithUiUpdate(item, 1);
    }

    //대상 장비 장착칸에 장비를 추가하는 메서드
    public void LoadEquipmentItem(EquipmentItemData item, int idx = 0)
    {
        equipmentItemSlot[idx] = item;
        equipmentItemSlot[idx].EquipItem(playerStatus);
    }

    public void LoadEquipmentItemFromInventory(EquipmentItemData equipData, int equipSlotIdx)
    {
        LoadEquipmentItem(equipData, equipSlotIdx);
        inventory.Remove(equipData);
    }

    //대상 장비 장착칸에 장비를 제거하는 메서드
    public void UnloadEquipmentItem(int idx = 0)
    {
        equipmentItemSlot[idx].UnEquipItem(playerStatus);
        AddItemsToInventory(equipmentItemSlot[idx], 1);
        equipmentItemSlot[idx] = dummyItemData;
    }

    //인벤토리에 있는 장비와 장착칸의 장비를 서로 교체하는 함수
    public void SwapEquippedItemWithInventory(int equippedSlotIdx, EquipmentItemData inventoryItemData)
    {
        UnloadEquipmentItem(equippedSlotIdx);
        LoadEquipmentItemFromInventory(inventoryItemData, equippedSlotIdx);
    }

    //장비 장착칸의 아이템 슬롯끼리 교체하는 함수
    public void SwapEquipmentItemSlots(int idx1, int idx2)
    {
        EquipmentItemData temp = equipmentItemSlot[idx1];
        equipmentItemSlot[idx1] = equipmentItemSlot[idx2];
        equipmentItemSlot[idx2] = temp;
    }

    //대상 아이템을 인벤토리에 추가하는 함수
    private bool AddItemsToInventoryWithUiUpdate(BasicItemData item, int amount)
    {

        if (inventory.ContainsKey(item))   //인벤토리에 있는 경우
        {
            if (inventory[item] + amount <= item.MaxAmount)
            {
                inventory[item] += amount;
                myInventoryUI.UpdateItemSlotAmount(item, inventory[item]);
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
            inventory.Add(item, amount);
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
        if (inventory.ContainsKey(item))   //인벤토리에 있는 경우
        {
            if (inventory[item] + amount <= item.MaxAmount)
            {
                inventory[item] += amount;
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
            inventory.Add(item, amount);
            return true;
        }
        else
        {
            Debug.Log("인벤토리에 빈 공간이 없습니다!");
            return false;
        }
    }
}
