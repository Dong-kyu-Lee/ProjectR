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

    private void Awake()
    {

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))    //퀵슬롯에 등록된 아이템 사용
        {
            //myInventory.UseQuickSlotItem();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            myInventory.UnloadEquipmentItem();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            myInventory.SwapEquipmentItemSlots(0, 4);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            var temp = myInventory.InventoryDict.Keys.First();  //디버깅을 위해 인벤토리의 key들을 배열로 반환한 것.
            myInventory.SwapEquippedItemWithInventory(1, temp as EquipmentItemData);
        }
    }

    //아이템을 인벤토리에 추가하는 메서드
    public bool AddItem(BasicItemData item, int amount)
    {
        return myInventory.AddItem(item, amount);
    }
}
