using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemUseController : MonoBehaviour
{
    [SerializeField]
    private Inventory myInventory;
    public Inventory MyInventory { get { return myInventory; } }

    //디버그용 변수들
    public int equippedSlotIndex1 = 0;
    public int equippedSlotIndex2 = 4;
    public int inventorySlotIndex = 0;

    private void Awake()
    {
        myInventory = transform.GetChild(0).GetComponent<Inventory>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))    //퀵슬롯에 등록된 아이템 사용
        {
            //myInventory.UseQuickSlotItem();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            myInventory.UnloadEquipmentItem(equippedSlotIndex1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //myInventory.SwapEquipmentItemSlots(0, 4);
            myInventory.SwapEquipmentItemSlots(equippedSlotIndex1, equippedSlotIndex2);
        }
    }

    //아이템을 인벤토리에 추가하는 메서드
    public bool AddItem(BasicItemData item, int amount)
    {
        return myInventory.AddItem(item, amount);
    }
}