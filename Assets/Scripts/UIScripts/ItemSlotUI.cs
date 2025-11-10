using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] protected BasicItemData dummyItemData;     //더미 아이템 데이터. 아이템 데이터가 없음을 나타낼 때 사용
    [SerializeField] protected BasicItemData nowItemData;       //현재 가지고 있는 아이템 데이터. 데이터가 없으면 더미 아이템 데이터로 설정
    [SerializeField] protected Image itemSlotImage;             //현재 슬롯에 들어가있는 아이템 이미지. 자식의 Image를 참조하여 이를 바꾸는 형태
    [SerializeField] protected InventoryUI parentUI;            //인벤토리 UI
    protected int slotIndex;                                    //자신의 슬롯의 순서를 나타내는 인덱스
    protected int itemCount = 0;                                //현재 슬롯의 아이템의 갯수
    [SerializeField] protected Text itemCountText;              //아이템 갯수를 표시할 텍스트

    //아이템 설명용
    private Button slotButton;                                  //버튼 컴포넌트
    private InventoryItemExplain explainUI;                     //아이템 설명 UI

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

    //자신의 슬롯의 초기화 함수
    public void Init(GameObject parent, int indexNumber)
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

        //버튼과 설명창 연결
        slotButton = itemSlotImage.GetComponent<Button>();
        if (slotButton != null)
        {
            explainUI = FindObjectOfType<InventoryItemExplain>();
            slotButton.onClick.AddListener(() => explainUI.OnItemSlotClicked(nowItemData));
        }

        isInitialized = true;
    }

    //자신의 아이템 데이터를 삽입하고 이미지와 갯수 텍스트를 설정 하는 메서드
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

    //아이템의 갯수 텍스트만 설정하는 메서드
    public void SetItemAmountData(int amount)
    {
        itemCount = amount;
        UpdateItemSpriteAndAmountText();
    }

    //자신의 슬롯의 아이템 이미지와 개수 텍스트를 업데이트하는 메서드
    public void UpdateItemSpriteAndAmountText()
    {
        itemSlotImage.sprite = nowItemData.ItemSprite;
        itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
    }

    //더미 아이템 데이터로 설정하고 자신의 슬롯의 아이템 이미지를 초기화하는 메서드
    public void DeleteItemData()
    {
        nowItemData = dummyItemData;
        itemCount = 0;
        UpdateItemSpriteAndAmountText();
    }

    //아이템 슬롯 UI의 데이터들 끼리 Swap하는 함수.
    //SetItemData()를 사용했기에 이미지와 아이템 갯수 텍스트까지 같이 업데이트됨.
    public void SwapItemData(ItemSlotUI targetSlot)
    {
        if (targetSlot == null) return;
        BasicItemData temp = nowItemData;
        int tempItemCount = itemCount;
        SetItemData(targetSlot.nowItemData, targetSlot.itemCount);
        targetSlot.SetItemData(temp, tempItemCount);
    }

    //자기 자신인 아이템 슬롯이 Drag가 시작되었을 때 호출되는 함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (nowItemData.ItemType == ItemType.DUMMY) return;  //자신이 비어있는 칸일 경우 드래그 안되게 방지

        parentUI.PreviewSlotUI.gameObject.SetActive(true);  //미리보기 Slot 활성화 및 이미지와 갯수 텍스트 설정
        parentUI.PreviewSlotUI.SetItemData(nowItemData, itemCount);
    }

    //Drag 중일때 호출되는 함수. 미리보기 슬롯의 위치 갱신
    public void OnDrag(PointerEventData eventData)
    {
        parentUI.PreviewSlotUI.transform.position = Input.mousePosition;
    }

    //다른 슬롯에서 출발해서 자신의 슬롯 위에 Drop이 되었을 때 호출.
    //EquipmentSlotUI 클래스가 이 함수를 재정의함.
    public virtual void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI targetSlotUI = eventData.pointerDrag.GetComponent<ItemSlotUI>(); //다른 ItemSlotUI
        if (targetSlotUI == null || targetSlotUI.NowItemData.ItemType == ItemType.DUMMY) return;

        // (인벤토리칸 <-> 장비칸) => 장비 스왑 / 언로드
        if (targetSlotUI is EquipmentSlotUI)
        {
            switch (nowItemData.ItemType)
            {
                case ItemType.EQUIPMENT: //(인벤토리칸 <-> 장비칸) => 장비 스왑
                    SwapItemData(targetSlotUI);
                    parentUI.PlayerInventory.SwapEquippedItemWithInventory(
                        targetSlotUI.SlotIndex, nowItemData as EquipmentItemData);
                    break;

                case ItemType.DUMMY: //(장비 -> 인벤토리) => 장비 언로드
                    parentUI.PlayerInventory.UnloadEquipmentItem((targetSlotUI as EquipmentSlotUI).SlotIndex);
                    SetItemData(targetSlotUI.NowItemData, targetSlotUI.itemCount);
                    targetSlotUI.DeleteItemData();
                    break;
            }
        }
        // 퀵슬롯은 인벤토리의 첫 번째 칸을 참조하므로, 별도 스왑/언로드 동작은 없음
        else if (targetSlotUI is QuickSlotUI)
        {
            return;
        }
        // 인벤토리칸 끼리 스왑
        else
        {
            SwapItemData(targetSlotUI);
        }

        if (parentUI.PlayerInventory != null)
            parentUI.PlayerInventory.UpdateQuickSlotReference();
    }

    //드래그를 끝냈을 때 미리보기 UI 슬롯 비활성화
    public void OnEndDrag(PointerEventData eventData)
    {
        parentUI.PreviewSlotUI.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (nowItemData != null && nowItemData.ItemType == ItemType.CONSUMABLE)
            {
                // 퀵슬롯은 인벤토리 첫 번째 칸을 참조하므로, 직접 사용만 수행
                parentUI.PlayerInventory.UseQuickSlotItem();
            }
        }
    }
}
