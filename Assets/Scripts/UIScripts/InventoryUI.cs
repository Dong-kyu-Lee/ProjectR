using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        if (equipSlotImgs != null) UpdateAllEquippedItemSlotImages();
        if (playerInventory.InventoryDict != null) UpdateAllInventorySlotImages();
    }

    //시작 전 초기화 함수
    public void Init()
    {
        PlayerItemUseController player = FindObjectOfType<PlayerItemUseController>();
        playerInventory = player.MyInventory;
        player.MyInventoryUI = this;
        GenerateEquippedItemSlot();
        InitiateAllItemsSlots();
    }

    //장비칸 UI를 생성하는 함수(LazyInstantiation)
    public void GenerateEquippedItemSlot()
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
    public void SetItemToUI(BasicItemData item, int amound)
    {
        switch (item.ItemType)
        {
            case ItemType.CONSUMABLE:
                SetInventorySlotData(item);
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
    }

    //특정 장비칸(slotIdx)의 이미지 갱신 함수
    public void UpdateEquippedItemSlotImage(int slotIdx)
    {
        equipSlotImgs[slotIdx].SetItemData(playerInventory.EquipmentItemSlot[slotIdx]);
    }

    //모든 장비칸 이미지 업데이트 함수
    public void UpdateAllEquippedItemSlotImages()
    {
        for (int i = 0; i < equipSlotImgs.Count; i++)
        {
            UpdateEquippedItemSlotImage(i);
        }
    }

    //해당 인벤토리 슬롯 데이터 반영 함수
    public void SetInventorySlotData(BasicItemData itemData)
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

    //특정 인벤토리칸 이미지 업데이트 함수
    public void UpdateInventorySlotImages(int slotRowIdx, int slotColIdx)
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
    public void InitiateAllItemsSlots()
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
