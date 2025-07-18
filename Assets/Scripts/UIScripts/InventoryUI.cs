using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    //장비칸 UI 관련 변수들
    [SerializeField]
    private GameObject equipSlotParentObj;                              //장비칸 UI의 부모 오브젝트
    [SerializeField] private EquipmentSlotUI[] equipSlotImgs = null;    //장비칸 UI 오브젝트들
    
    //인벤토리 UI 관련 변수
    [SerializeField] private GameObject inventorySlotParentObj;         //인벤토리 슬롯 UI의 부모 오브젝트
    [SerializeField] private ItemSlotUI[] inventorySlotImgs;            //인벤토리 슬롯 UI 오브젝트들

    //퀵슬롯 UI 관련 변수
    [SerializeField] private ItemSlotUI quickSlotImg;                   //퀵슬롯 UI
    public ItemSlotUI QuickSlotImg { get { return quickSlotImg; } }

    //인벤토리, 장비창 UI 공용 변수
    [SerializeField] private ItemSlotUI previewSlotUI;                  //아이템 드래그시 생기는 미리보기 UI 오브젝트
    public ItemSlotUI PreviewSlotUI { get { return previewSlotUI; } }

    //플레이어 인벤토리
    [SerializeField] private Inventory playerInventory;                 //플레이어의 인벤토리
    public Inventory PlayerInventory { get { return playerInventory; } }


    //시작 전 초기화 함수. ChracterInfo의 Awake()에서 호출됨
    public void Init()
    {
        playerInventory = GameManager.Instance.CurrentPlayer.transform.GetChild(0).GetComponent<Inventory>();
        playerInventory.MyInventoryUI = this;

        InitiateAllItemsSlots();

        Inventory.OnItemAdded += HandleItemAdded;
    }
    private void HandleItemAdded(BasicItemData item, int amount)
    {
        SetItemToUI(item, amount);
    }
    //인벤토리 & 장비의 모든 슬롯 초기화 함수
    private void InitiateAllItemsSlots()
    {
        //미리보기 슬롯 초기화. -1은 해당 UI의 index가 의미 없음을 나타냄.
        previewSlotUI.Init(gameObject, -1);
        //장비칸 슬롯 초기화
        equipSlotImgs = equipSlotParentObj.transform.GetComponentsInChildren<EquipmentSlotUI>();
        for (int i = 0; i < equipSlotImgs.Length; i++)
        {
            equipSlotImgs[i].Init(gameObject, i);
        }

        inventorySlotImgs = inventorySlotParentObj.transform.GetComponentsInChildren<ItemSlotUI>();

        //인벤토리 슬롯 초기화
        for (int i = 0; i < inventorySlotImgs.Length; i++)
        {
            inventorySlotImgs[i].Init(gameObject, i);
            Debug.Log($"[InitCheck] Init 호출됨: {inventorySlotImgs[i].gameObject.name}");
        }
        //퀵슬롯 초기화. -1은 해당 UI의 index가 의미 없음을 나타냄.
        QuickSlotImg.Init(gameObject, -1);
    }

    //획득한 아이템을 UI에 반영하는 함수.
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

    //획득한 장비 아이템의 데이터를 비어있는 장비칸 UI에 삽입 및 UI 업데이트를 하는 함수
    private void SetEquippedItemSlotData(EquipmentItemData itemData)
    {
        for (int i = 0; i < equipSlotImgs.Length; i++) 
        {
            if (equipSlotImgs[i].NowItemData.ItemType == ItemType.DUMMY)    //비어있는 장비칸을 검색해 삽입
            {
                equipSlotImgs[i].SetItemData(itemData, 1);
                return;
            }
        }

        SetInventorySlotData(itemData, 1);
    }
    //아이템의 데이터를 비어있는 인벤토리 슬롯 UI에 삽압하는 함수
    public void SetInventorySlotData(BasicItemData itemData, int amount)
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

    //퀵슬롯 UI에 아이템 데이터를 삽입 및 UI 업데이트 하는 함수
    public void SetQuickSLotItemData(BasicItemData itemData, int amount)
    {
        if (!quickSlotImg.IsInitialized)
            quickSlotImg.Init(gameObject, -1);

        quickSlotImg.SetItemData(itemData, amount);
    }
    private void OnDestroy()
    {
        Inventory.OnItemAdded -= HandleItemAdded;
    }
}
