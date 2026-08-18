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

    // 더블 클릭(중복 실행) 방지용 타이머
    private float lastRightClickTime = 0f;

    //자신의 슬롯의 초기화 함수
    public virtual void Init(GameObject parent, int indexNumber)
    {
        parentUI = parent.GetComponent<InventoryUI>();
        nowItemData = dummyItemData;

        itemSlotImage.sprite = nowItemData.ItemSprite;
        slotIndex = indexNumber;

        //버튼과 설명창 연결
        if (itemSlotImage != null)
        {
            slotButton = itemSlotImage.GetComponent<Button>();
            if (slotButton != null)
            {
                // 기존 Button의 좌클릭 이벤트를 모두 지우고 클릭 중계기(Forwarder)에서 한 번에 처리합니다.
                slotButton.onClick.RemoveAllListeners();
            }

            // EventTrigger 대신, 드래그를 방해하지 않는 커스텀 클릭 중계기를 붙여줍니다.
            EventTrigger trigger = itemSlotImage.gameObject.GetComponent<EventTrigger>();
            if (trigger != null) Destroy(trigger); // 혹시 남아있는 악성 EventTrigger 파괴

            SlotClickForwarder forwarder = itemSlotImage.gameObject.GetComponent<SlotClickForwarder>();
            if (forwarder == null)
            {
                forwarder = itemSlotImage.gameObject.AddComponent<SlotClickForwarder>();
            }
            forwarder.parentSlot = this; // 자식(이미지)이 부모(현재 스크립트)를 기억하게 연결

            isInitialized = true;
        }
    }

    //자신의 아이템 데이터를 삽입하고 이미지와 갯수 텍스트를 설정 하는 메서드
    public void SetItemData(BasicItemData itemData, int amount = 1)
    {
        nowItemData = itemData;
        itemCount = amount;

        if (itemSlotImage != null)
            itemSlotImage.sprite = nowItemData.ItemSprite;

        if (itemCountText != null)
        {
            if (nowItemData.ItemType == ItemType.CONSUMABLE)
                itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
            else
                itemCountText.text = "";
        }
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

        if (itemCountText != null)
        {
            if (nowItemData.ItemType == ItemType.CONSUMABLE)
                itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
            else
                itemCountText.text = "";
        }
    }

    //더미 아이템 데이터로 설정하고 자신의 슬롯의 아이템 이미지를 초기화하는 메서드
    public void DeleteItemData()
    {
        nowItemData = dummyItemData;
        itemCount = 0;
        UpdateItemSpriteAndAmountText();
    }

    //아이템 슬롯 UI의 데이터들 끼리 Swap하는 함수.
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
        if (nowItemData.ItemType == ItemType.DUMMY) return;

        parentUI.PreviewSlotUI.gameObject.SetActive(true);
        parentUI.PreviewSlotUI.SetItemData(nowItemData, itemCount);
    }

    //Drag 중일때 호출되는 함수. 미리보기 슬롯의 위치 갱신
    public void OnDrag(PointerEventData eventData)
    {
        parentUI.PreviewSlotUI.transform.position = Input.mousePosition;
    }

    //다른 슬롯에서 출발해서 자신의 슬롯 위에 Drop이 되었을 때 호출.
    public virtual void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI targetSlotUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (targetSlotUI == null || targetSlotUI == this || parentUI.PlayerInventory == null) return;
        if (targetSlotUI.NowItemData.ItemType == ItemType.DUMMY) return;

        bool needsRefresh = false;

        // 장비창에서 인벤토리로 드롭된 경우
        if (targetSlotUI is EquipmentSlotUI)
        {
            switch (nowItemData.ItemType)
            {
                case ItemType.EQUIPMENT:
                    parentUI.PlayerInventory.SwapEquippedItemWithInventory(
                        targetSlotUI.SlotIndex, this.slotIndex);
                    needsRefresh = true;
                    break;

                case ItemType.DUMMY:
                    parentUI.PlayerInventory.UnloadEqToInv_NoRefresh(
                        (targetSlotUI as EquipmentSlotUI).SlotIndex, this.slotIndex);
                    needsRefresh = true;
                    break;
            }
        }
        else // 인벤토리 안에서 자리 바꾸기
        {
            parentUI.PlayerInventory.SwapInventorySlots(slotIndex, targetSlotUI.SlotIndex);
            needsRefresh = true;
        }

        if (needsRefresh)
        {
            parentUI.RefreshInventoryUI();
        }
    }

    //드래그를 끝냈을 때 미리보기 UI 슬롯 비활성화
    public void OnEndDrag(PointerEventData eventData)
    {
        parentUI.PreviewSlotUI.gameObject.SetActive(false);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (nowItemData == null || nowItemData.ItemType == ItemType.DUMMY) return;

        if (explainUI == null)
            explainUI = FindObjectOfType<InventoryItemExplain>(true);

        // [좌클릭] : 아이템 상세 정보 표시
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (explainUI != null)
            {
                explainUI.ShowPanel(nowItemData);
            }
        }
        // [우클릭] : 타입에 따른 분기 (비교 OR 사용)
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 더블 클릭 방지
            if (Time.time - lastRightClickTime < 0.1f) return;
            lastRightClickTime = Time.time;

            if (nowItemData.ItemType == ItemType.CONSUMABLE)
            {
                //parentUI.PlayerInventory.UseInventoryItem(slotIndex);
                return;
            }
            else if (nowItemData.ItemType == ItemType.EQUIPMENT)
            {
                EquipmentItemData equipData = nowItemData as EquipmentItemData;
                if (equipData != null && explainUI != null)
                {
                    bool isEquippedItem = this is EquipmentSlotUI;
                    explainUI.AddCompareItem(equipData, isEquippedItem);
                }
            }
        }
    }
}

// [새로 추가된 클래스] 자식 오브젝트의 클릭 이벤트를 부모로 넘겨주는 중계기 역할
public class SlotClickForwarder : MonoBehaviour, IPointerClickHandler
{
    public ItemSlotUI parentSlot;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (parentSlot != null)
        {
            parentSlot.OnPointerClick(eventData);
        }
    }
}