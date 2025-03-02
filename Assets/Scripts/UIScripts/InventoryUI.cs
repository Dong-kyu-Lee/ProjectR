using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    //장비칸 UI 관련 변수들
    [SerializeField]
    private GameObject equipSlotParentObj;
    [SerializeField]
    private GameObject itemSlotPref;
    //private List<ItemSlotUI> equipSlotImgs = null;
    private List<EquipmentSlotUI> equipSlotImgs = null;
    
    //인벤토리 UI 관련 변수
    [SerializeField]
    private GameObject[] inventorySlotParentObj;
    [SerializeField] private ItemSlotUI[] inventorySlotImgs;
    public ItemSlotUI[] InventorySlotImgs { get { return inventorySlotImgs; } }

    //인벤토리, 장비창 UI 공용 변수
    [SerializeField] private ItemSlotUI previewSlotUI;
    public ItemSlotUI PreviewSlotUI { get { return previewSlotUI; } }

    //플레이어 인벤토리
    [SerializeField]
    private Inventory playerInventory;
    public Inventory PlayerInventory { get { return playerInventory; } }

    private void OnEnable()
    {
        //if (equipSlotImgs != null) UpdateAllEquippedItemSlotImages();
        //if (playerInventory.InventoryDict != null) UpdateAllInventorySlotImages();
    }

    //시작 전 초기화 함수
    public void Init()
    {
        playerInventory = FindObjectOfType<Inventory>();
        playerInventory.MyInventoryUI = this;

        GenerateEquippedItemSlot();
        InitiateAllItemsSlots();
    }

    //장비칸 UI를 생성하는 함수(LazyInstantiation)
    private void GenerateEquippedItemSlot()
    {
        equipSlotImgs = new List<EquipmentSlotUI>();
        for (int i = 0; i < playerInventory.MaxEquipSlot; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPref, equipSlotParentObj.transform);
            itemSlot.GetComponent<RectTransform>().localPosition
                = new Vector3(-90 + (i % 3) * 90, -110 * (i / 3) + 20, 0);  //3은 가로행 수(row)
            itemSlot.GetComponentInChildren<EquipmentSlotUI>().SlotIndex = i;
            equipSlotImgs.Add(itemSlot.GetComponentInChildren<EquipmentSlotUI>());
        }
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

    private void SetEquippedItemSlotData(EquipmentItemData itemData)
    {
        for (int i = 0; i < equipSlotImgs.Count; i++) 
        {
            if (equipSlotImgs[i].NowItemData.ItemType == ItemType.DUMMY)
            {
                equipSlotImgs[i].SetItemData(itemData);
                return;
            }
        }
        SetInventorySlotData(itemData, 1);
    }

    //특정 장비칸(slotIdx)의 이미지 갱신 함수
    private void UpdateEquippedItemSlotImage(int slotIdx)
    {
        equipSlotImgs[slotIdx].SetItemData(playerInventory.EquipmentItemSlot[slotIdx]);
    }

    //모든 장비칸 이미지 업데이트 함수
    private void UpdateAllEquippedItemSlotImages()
    {
        for (int i = 0; i < equipSlotImgs.Count; i++)
        {
            UpdateEquippedItemSlotImage(i);
        }
    }

    //아이템의 데이터를 비어있는 인벤토리 슬롯에 삽압하는 함수
    public void SetInventorySlotData(BasicItemData itemData, int amount) //amount는 추후에 사용 예정
    {
        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            if (inventorySlotImgs[i].NowItemData.ItemType == ItemType.DUMMY)
            {
                inventorySlotImgs[i].SetItemData(itemData);
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
                Debug.Log("아이템 갯수 UI에 반영");
                return;
            }
        }
    }

    //특정 인벤토리칸 이미지 업데이트 함수
    private void UpdateInventorySlotImages(int slotRowIdx, int slotColIdx)
    {
        inventorySlotParentObj[slotColIdx].transform.GetChild(slotRowIdx).GetComponent<ItemSlotUI>().UpdateItemSprite();
    }

    //모든 인벤토리 슬롯 이미지 업데이트 함수
    public void UpdateAllInventorySlotImages()
    {
        int row = 0;
        int col = 0;
        foreach (var item in playerInventory.InventoryDict)
        {
            UpdateInventorySlotImages(row, col);
            if (row < inventorySlotParentObj[col].transform.childCount)
            {
                row++;
            }
            else
            {
                row = 0;
                col++;
            }
        }
    }


    //시작 전 인벤토리 & 장비의 모든 슬롯 초기화 함수
    private void InitiateAllItemsSlots()
    {
        previewSlotUI.Init(gameObject, -1);
        for (int i = 0; i < equipSlotImgs.Count; i++)
        {
            equipSlotImgs[i].Init(gameObject, i);
        }

        inventorySlotImgs = inventorySlotParentObj[0].transform.GetComponentsInChildren<ItemSlotUI>();
        for(int i = 0; i< inventorySlotImgs.Length; i++)
        {
            inventorySlotImgs[i].Init(gameObject, i);
        }        
    }
}
