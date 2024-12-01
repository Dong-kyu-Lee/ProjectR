using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private ConsumableItemData quickSlot = null;
    private int quickSlotItemAmount = 0;
    [SerializeField]
    private Dictionary<ConsumableItemData, int> inventory;  //인벤토리 딕셔너리<아이템정보, 갯수>
    public int maxSlotSize = 6;
    
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
    public Dictionary<ConsumableItemData, int> InventoryList { get { return inventory; } }
    public int MaxSlotSize { get { return maxSlotSize; } }

    private void Awake()
    {
        inventory = new Dictionary<ConsumableItemData, int>();
    }

    //퀵슬롯에 있는 아이템을 사용하는 메서드
    public void UseQuickSlotItem(GameObject player)
    {
        if(quickSlot)
        {
            quickSlot.ActivateItemEffect(player);
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
    public void LoadToQuickSlot(ConsumableItemData item, int amount = 1)
    {
        quickSlot = item;
        quickSlotItemAmount += amount;
    }

    //아이템을 인벤토리에 추가하는 메서드 
    public bool AddItem(ConsumableItemData item, int amount = 1)
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
}
