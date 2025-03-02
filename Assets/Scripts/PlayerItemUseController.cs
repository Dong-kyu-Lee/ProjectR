using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemUseController : MonoBehaviour
{
    [SerializeField]
    private Inventory myInventory;
    public Inventory MyInventory { get { return myInventory; } }

    [SerializeField]
    private float itemTakeRange = 1.0f;  //아이템을 줍는 범위

    private void Awake()
    {
        myInventory = transform.GetChild(0).GetComponent<Inventory>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))    //퀵슬롯에 등록된 아이템 사용
        {
            myInventory.UseQuickSlotItem();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryTakeNearItem();
        }
    }

    //필드의 아이템을 획득하는 함수
    public void TryTakeNearItem()
    {
        RaycastHit2D itemHit = Physics2D.CircleCast(transform.position, itemTakeRange, Vector2.zero, 0, LayerMask.GetMask("Item"));
        if (itemHit)
        {
            FieldItem fieldItem = itemHit.collider.GetComponent<FieldItem>();
            
            if (myInventory.AddItem(fieldItem.MyItemData, fieldItem.Amount))
            {
                Destroy(itemHit.transform.gameObject);
            }
        }
    }
}