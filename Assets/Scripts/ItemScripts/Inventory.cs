using System.Collections;
using System.Collections.Generic;
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
    //[SerializeField] private EquipmentItemData EquipmentItemSlot;
    [SerializeField] private int maxEquipSlot = 6;
    [SerializeField] private EquipmentItemData[] equipmentItemSlot;
    
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
    public Dictionary<BasicItemData, int> InventoryList { get { return inventory; } }
    public int MaxInventorySlot { get { return maxInventorySlot; } }

    private void Awake()
    {
        inventory = new Dictionary<BasicItemData, int>();
        playerStatus = GetComponentInParent<PlayerStatus>();
        equipmentItemSlot = new EquipmentItemData[maxEquipSlot];
    }

    private void OnDestroy()
    {
        //equipmentItemSlot.Free();
    }

    //퀵슬롯에 있는 아이템을 사용하는 메서드
    public void UseQuickSlotItem()
    {
        if (quickSlot)
        {
            quickSlot.ActivateItemEffect(playerStatus);
            QuickSlotItemAmount--;
            Debug.Log(quickSlotItemAmount);

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
                Debug.Log("장비 추가 함수 실행");
                return AddEquipmentItem(item as EquipmentItemData);
            default:
                Debug.Log("아이템이 어떠한 클래스 타입이랑도 매칭되지 않음");
                return false;
        }
    }

    private bool AddConsumableItem(ConsumableItemData item, int amount)
    {
        if (!quickSlot || quickSlot == item) //퀵슬롯에 아이템이 없거나 같은 아이템이 로드된 경우
        {
            LoadToQuickSlot(item, amount);
            return true;
        }

        return AddItemsToInventory(item, amount);
    }

    //인벤토리에 장비를 추가하는 함수
    private bool AddEquipmentItem(EquipmentItemData item)
    {
        for(int i = 0; i < equipmentItemSlot.Length; i++)
        {
            if (equipmentItemSlot[i] == null)
            {
                LoadEquipmentItem(item, i);
                return true;
            }
        }

        return AddItemsToInventory(item, 1);
    }

    //대상 장비 장착칸에 장비를 추가하는 메서드
    private void LoadEquipmentItem(EquipmentItemData item, int idx = 0)
    {
        equipmentItemSlot[idx] = item;
        Debug.Log("장비 장착함");
        equipmentItemSlot[idx].EquipItem(playerStatus);
    }

    //대상 장비 장착칸에 장비를 제거하는 메서드
    public void UnloadEquipmentItem(int idx = 0)
    {
        if (equipmentItemSlot != null)
        {
            equipmentItemSlot[idx].UnEquipItem(playerStatus);
            AddEquipmentItem(equipmentItemSlot[idx]);
            Debug.Log("장비 해제함");
            equipmentItemSlot[idx] = null;
        }
    }

    //대상 아이템을 인벤토리에 추가하는 함수
    private bool AddItemsToInventory(BasicItemData item, int amount)
    {
        if (inventory.ContainsKey(item))   //인벤토리에 있는 경우
        {
            if(inventory[item] + amount <= item.MaxAmount)
            {
                inventory[item] += amount;
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (inventory.Count <= maxInventorySlot)
        {
            inventory.Add(item, amount);
            return true;
        }
        return false;
    }

    public void GetMyInventoryStatus()  //디버깅용 인벤토리 확인 함수
    {
        foreach (var i in inventory)
        {
            Debug.Log("아이템 이름 : " + i.Key.ItemName + " 아이템 수량 : " + i.Value);
        }
    }
}
