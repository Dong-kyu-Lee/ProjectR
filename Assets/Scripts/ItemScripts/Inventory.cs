using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //소모품 관련 칸
    private ConsumableItemData quickSlot = null;
    private int quickSlotItemAmount = 0;
    [SerializeField]
    private Dictionary<BasicItemData, int> inventory;  //인벤토리 딕셔너리<아이템정보, 갯수>
    
    //장비 관련 칸
    [SerializeField]
    private EquipmentItemData EquipmentItemSlot;
    [SerializeField] 
    public int maxSlotSize = 6;
    private PlayerStatus playerStatus;
    
    public int QuickSlotItemAmount
    { 
        get { return quickSlotItemAmount; }
        set {
            quickSlotItemAmount = value;
            if (quickSlotItemAmount < 0) quickSlotItemAmount = 0;
        }
    }
    public ConsumableItemData QuickSlot { get { return quickSlot; } }
    public int QuickSlotAmount { get { return quickSlotItemAmount; } }
    public Dictionary<BasicItemData, int> InventoryList { get { return inventory; } }
    public int MaxSlotSize { get { return maxSlotSize; } }

    private void Awake()
    {
        inventory = new Dictionary<BasicItemData, int>();
        playerStatus = GetComponentInParent<PlayerStatus>();
    }

    //퀵슬롯에 있는 아이템을 사용하는 메서드
    public void UseQuickSlotItem()
    {
        if(quickSlot)
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
        switch(item.ItemType)
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

        if (inventory.ContainsKey(item))   //인벤토리에 있는 경우
        {
            inventory[item] += amount;
            return true;
        }
        else
        {
            if (inventory.Count <= maxSlotSize)
            {
                inventory.Add(item, amount);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //인벤토리에 장비를 추가하는 함수
    private bool AddEquipmentItem(EquipmentItemData item)
    {
        if(EquipmentItemSlot == null)
        {
            LoadEquipmentItem(item);
            return true;

        }
        else
        {
            if (inventory.ContainsKey(item))
            {
                Debug.Log("이미 인벤토리에 존재하는 아이템입니다.");
                return false;
            }
            else {
                inventory.Add(item, 1);
                return true;
            }
        }
    }

    //장비 장착칸에 장비를 추가하는 메서드
    private void LoadEquipmentItem(EquipmentItemData item)
    {
        EquipmentItemSlot = item;
        Debug.Log("장비 장착함");
        EquipmentItemSlot.EquipItem(playerStatus);
    }

    public void UnloadEquipmentItem()
    {
        if(EquipmentItemSlot != null)
        {
            EquipmentItemSlot.UnEquipItem(playerStatus);
            AddEquipmentItem(EquipmentItemSlot);
            Debug.Log("장비 해제함");
            EquipmentItemSlot = null;
        }

    }
}
