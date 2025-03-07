using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] protected BasicItemData dummyItemData;   //더미 아이템 데이터(= Null ItemData)
    [SerializeField] protected BasicItemData nowItemData; //현재 아이템 데이터
    [SerializeField] protected Image itemSlotImage;
    [SerializeField] protected InventoryUI parentUI;
    protected int slotIndex;
    protected int itemCount = 0;
    [SerializeField] protected Text itemCountText;
    public Image ItemSlotImage { get; set; }
    public BasicItemData NowItemData {
        get => nowItemData;
        set => nowItemData = value; 
    }
    public int SlotIndex { get => slotIndex; set => slotIndex = value; }
    public int ItemCount { get => itemCount; }

    public void Init(GameObject parent, int indexNumber)
    {
        parentUI = parent.GetComponent<InventoryUI>();
        nowItemData = dummyItemData;
        itemSlotImage = transform.GetChild(0).GetComponent<Image>();
        itemSlotImage.sprite = nowItemData.ItemSprite;
        slotIndex = indexNumber;
        itemCountText = transform.GetChild(1).GetComponent<Text>();
    }

    //자신의 아이템 데이터를 설정하는 메서드
    public void SetItemData(BasicItemData itemData, int amount) 
    {
        nowItemData = itemData;
        itemCount = amount;
        UpdateItemSprite();
    }

    //자신의 슬롯의 아이템 이미지를 업데이트하는 메서드
    public void UpdateItemSprite()
    {
        itemSlotImage.sprite = nowItemData.ItemSprite;
        itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
    }

    //더미 아이템 데이터로 설정하고 자신의 슬롯의 아이템 이미지를 초기화하는 메서드
    public void DeleteItemData()
    {
        nowItemData = dummyItemData;
        UpdateItemSprite();
    }

    public void SwapItemData(ItemSlotUI targetSlot)
    {
        if(targetSlot == null) return;
        BasicItemData temp = nowItemData;
        int tempItemCount = itemCount;
        SetItemData(targetSlot.nowItemData,targetSlot.itemCount);
        targetSlot.SetItemData(temp, tempItemCount);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(nowItemData.ItemType == ItemType.DUMMY) return;

        parentUI.PreviewSlotUI.gameObject.SetActive(true);
        parentUI.PreviewSlotUI.SetItemData(nowItemData, itemCount);
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentUI.PreviewSlotUI.transform.position = Input.mousePosition;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI targetSlotUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();
        if (targetSlotUI is EquipmentSlotUI)
        {
            if (nowItemData.ItemType == ItemType.EQUIPMENT)
            {
                SwapItemData(targetSlotUI);
                parentUI.PlayerInventory.SwapEquippedItemWithInventory(targetSlotUI.SlotIndex, nowItemData as EquipmentItemData);
            }
            else if(nowItemData.ItemType == ItemType.DUMMY)
            {
                parentUI.PlayerInventory.UnloadEquipmentItem((targetSlotUI as EquipmentSlotUI).SlotIndex);
                SetItemData(targetSlotUI.NowItemData,targetSlotUI.itemCount);
                targetSlotUI.DeleteItemData();
            }
        }
        else
        {
            SwapItemData(targetSlotUI);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentUI.PreviewSlotUI.gameObject.SetActive(false);
    }
}
