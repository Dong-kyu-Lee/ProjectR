using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    //장비칸 UI 관련 변수들
    [SerializeField]
    private GameObject equipSlotParentObj;
    [SerializeField] private EquipmentSlotUI[] equipSlotImgs = null;
    
    //인벤토리 UI 관련 변수
    [SerializeField] private GameObject inventorySlotParentObj;
    [SerializeField] private ItemSlotUI[] inventorySlotImgs;

    //퀵슬롯 UI 관련 변수
    [SerializeField] private ItemSlotUI quickSlotImg;
    public ItemSlotUI QuickSlotImg { get { return quickSlotImg; } }

    //인벤토리, 장비창 UI 공용 변수
    [SerializeField] private ItemSlotUI previewSlotUI;
    public ItemSlotUI PreviewSlotUI { get { return previewSlotUI; } }

    //플레이어 인벤토리
    [SerializeField] private Inventory playerInventory;
    public Inventory PlayerInventory { get { return playerInventory; } }


    //시작 전 초기화 함수
    public void Init()
    {
        playerInventory = GameManager.Instance.CurrentPlayer.transform.GetChild(0).GetComponent<Inventory>();
        playerInventory.MyInventoryUI = this;

        InitiateAllItemsSlots();
    }

    //인벤토리 & 장비의 모든 슬롯 초기화 함수
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

    //획득한 아이템을 UI에 반영하는 함수
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
            default:
                break;
        }
    }

    //획득한 장비 아이템을 장비칸에 삽입하는 함수
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

    //아이템의 데이터를 비어있는 인벤토리 슬롯에 삽압하는 함수
    public void SetInventorySlotData(BasicItemData itemData, int amount) //amount는 추후에 사용 예정
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

    //인벤토리 슬롯에 있는 아이템의 갯수를 업데이트 하는 함수
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

    //퀵슬롯UI에 아이템을 삽입하는 함수
    public void SetQuickSLotItemData(BasicItemData itemData, int amount)
    {
        quickSlotImg.SetItemData(itemData, amount);
    }

    //해당 아이템이 들어있는 인벤토리 슬롯 UI의 데이터를 삭제하는 함수. 아무도 사용 안하면 삭제 예정
    public bool DeleteInventoryItemData(BasicItemData targetData)
    {
        for(int i = 0; i < inventorySlotImgs.Length; i++)
        {
            if (inventorySlotImgs[i].NowItemData == targetData)
            {
                inventorySlotImgs[i].DeleteItemData();
                return true;
            }
        }
        return false;
    }
}
