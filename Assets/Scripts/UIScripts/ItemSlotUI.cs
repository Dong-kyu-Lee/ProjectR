using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] protected BasicItemData dummyItemData;
    [SerializeField] protected BasicItemData nowItemData;
    [SerializeField] protected Image itemSlotImage;
    [SerializeField] protected InventoryUI parentUI;
    protected int slotIndex;
    protected int itemCount = 0;
    [SerializeField] protected Text itemCountText;

    private Button slotButton;
    private InventoryItemExplain explainUI;

    public Image ItemSlotImage { get; set; }
    public BasicItemData NowItemData
    {
        get => nowItemData;
        set => nowItemData = value;
    }
    public int SlotIndex { get => slotIndex; set => slotIndex = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }

    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    public virtual void Init(GameObject parent, int indexNumber)
    {
        parentUI = parent.GetComponent<InventoryUI>();
        nowItemData = dummyItemData;

        itemSlotImage = transform
            .GetComponentsInChildren<Image>(true)
            .FirstOrDefault(img => img.gameObject.name == "ItemImg");

        itemCountText = transform
            .GetComponentsInChildren<Text>(true)
            .FirstOrDefault(txt => txt.gameObject.name == "ItemCountText");

        itemSlotImage.sprite = nowItemData.ItemSprite;
        slotIndex = indexNumber;

        slotButton = itemSlotImage.GetComponent<Button>();
        if (slotButton != null)
        {
            explainUI = FindObjectOfType<InventoryItemExplain>();
            slotButton.onClick.AddListener(() => explainUI.OnItemSlotClicked(nowItemData));
        }

        isInitialized = true;
    }

    public void SetItemData(BasicItemData itemData, int amount = 1)
    {
        Debug.Log(itemData.ItemName);
        nowItemData = itemData;
        itemCount = amount;

        if (itemSlotImage != null)
            itemSlotImage.sprite = nowItemData.ItemSprite;

        if (itemCountText != null)
            itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
    }

    public void SetItemAmountData(int amount)
    {
        itemCount = amount;
        UpdateItemSpriteAndAmountText();
    }

    public void UpdateItemSpriteAndAmountText()
    {
        itemSlotImage.sprite = nowItemData.ItemSprite;
        itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
    }

    public void DeleteItemData()
    {
        nowItemData = dummyItemData;
        itemCount = 0;
        UpdateItemSpriteAndAmountText();
    }

    public void SwapItemData(ItemSlotUI targetSlot)
    {
        if (targetSlot == null) return;
        BasicItemData temp = nowItemData;
        int tempItemCount = itemCount;
        SetItemData(targetSlot.nowItemData, targetSlot.itemCount);
        targetSlot.SetItemData(temp, tempItemCount);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (nowItemData.ItemType == ItemType.DUMMY) return;

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
        if (targetSlotUI == null || targetSlotUI.NowItemData.ItemType == ItemType.DUMMY) return;

        // (인벤토리칸 <-> 장비칸) => 장비 스왑 / 언로드
        if (targetSlotUI is EquipmentSlotUI)
        {
            switch (nowItemData.ItemType)
            {
                case ItemType.EQUIPMENT:
                    //데이터 스왑을 먼저 호출
                    parentUI.PlayerInventory.SwapEquippedItemWithInventory(
                        targetSlotUI.SlotIndex, nowItemData as EquipmentItemData);
                    SwapItemData(targetSlotUI);
                    break;

                case ItemType.DUMMY:
                    parentUI.PlayerInventory.UnloadEquipmentItem((targetSlotUI as EquipmentSlotUI).SlotIndex);
                    SetItemData(targetSlotUI.NowItemData, targetSlotUI.itemCount);
                    targetSlotUI.DeleteItemData();
                    break;
            }
        }
        else if (targetSlotUI is QuickSlotUI)
        {
            return;
        }
        // 인벤토리칸 끼리 스왑
        else
        {
            // UI 스왑 전에 백엔드 데이터 스왑 먼저 실행
            parentUI.PlayerInventory.SwapInventorySlots(slotIndex, targetSlotUI.SlotIndex);
            SwapItemData(targetSlotUI);
        }

        if (parentUI.PlayerInventory != null)
            parentUI.PlayerInventory.UpdateQuickSlotReference();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentUI.PreviewSlotUI.gameObject.SetActive(false);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (nowItemData != null && nowItemData.ItemType == ItemType.CONSUMABLE)
            {
                // 퀵슬롯(0번)이 아닌 '자기 자신(slotIndex)'의 아이템을 사용
                parentUI.PlayerInventory.UseInventoryItem(slotIndex);
            }
        }
    }
}