using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    //장비칸 UI 관련 변수들
    [SerializeField]
    private GameObject equipSlotParentObj;
    [SerializeField]
    private GameObject itemSlotPref;
    private List<ItemSlotUI> equipSlotImgs = null;

    //인벤토리 UI 관련 변수
    [SerializeField]
    private GameObject[] inventorySlotParentObj;

    //플레이어 인벤토리
    [SerializeField]
    private Inventory playerInventory;

    private void OnEnable()
    {
        if (equipSlotImgs != null) UpdateAllEquippedItemSlotImages();
        if (playerInventory.InventoryDict != null) UpdateAllInventorySlotImages();
    }

    //시작 전 초기화 함수
    public void Init()
    {
        GenerateEquippedItemSlot();
        InitiateAllItemsSlots();
    }

    //장비칸 UI를 생성하는 함수(LazyInstantiation)
    public void GenerateEquippedItemSlot()
    {
        equipSlotImgs = new List<ItemSlotUI>();
        for (int i = 0; i < playerInventory.MaxEquipSlot; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPref, equipSlotParentObj.transform);
            itemSlot.GetComponent<RectTransform>().localPosition
                = new Vector3(-90 + (i % 3) * 90, -110 * (i / 3) + 20, 0);  //3은 가로행 수(row)

            equipSlotImgs.Add(itemSlot.transform.GetChild(0).GetComponent<ItemSlotUI>());
        }
    }

    //특정 장비칸 데이터 삽입 함수
    public void SetEquippedItemSlotData(int slotIdx, BasicItemData itemData)
    {
        equipSlotImgs[slotIdx].SetItemData(itemData);
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
    public void SetInventorySlotData(BasicItemData itemData, int slotRowIdx, int slotColIdx)
    {
        inventorySlotParentObj[slotColIdx].transform.GetChild(slotRowIdx).GetComponent<ItemSlotUI>().SetItemData(itemData);
        //inventorySlotParentObj[slotRowIdx].transform.GetChild(slotColIdx).GetComponent<Image>().sprite = targetSprite.ItemSprite;
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

    //인벤토리에 있는 모든 아이템 데이터를 인벤토리 슬롯 UI에 삽입하는 함수
    public void SetAllInventorySlotItemDatas()
    {
        int row = 0;
        int col = 0;

        foreach (var item in playerInventory.InventoryDict)
        {
            SetInventorySlotData(item.Key, row, col);
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
        for (int i = 0; i < equipSlotImgs.Count; i++)
        {
            equipSlotImgs[i].Init();
        }
        for (int i = 0; i < inventorySlotParentObj[0].transform.childCount; i++)
        {
            inventorySlotParentObj[0].transform.GetChild(i).GetComponent<ItemSlotUI>().Init();
        }
    }
}
