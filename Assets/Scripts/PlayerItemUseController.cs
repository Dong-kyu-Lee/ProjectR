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
    private Inventory myInventory;  //플레이어의 인벤토리
    public Inventory MyInventory { get { return myInventory; } }

    [SerializeField]
    private float itemTakeRange = 1.0f;  //아이템을 줍는 범위

    private void Awake()
    {
        // [수정/제거] myInventory = transform.GetChild(0).GetComponent<Inventory>();
        // [추가] 계층 구조(순서)가 변경되어도 안전하게 Inventory 컴포넌트를 가져오도록 수정
        myInventory = GetComponentInChildren<Inventory>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))    //퀵슬롯에 등록된 아이템 사용
        {
            myInventory.UseQuickSlotItem();
        }
        if (Input.GetKeyDown(KeyCode.E))    //아이템 획득
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
            var item = fieldItem.MyItemData;

            // 그 외 소비아이템은 인벤토리에 추가
            myInventory.AddItem(item, fieldItem.Amount);
            Destroy(itemHit.transform.gameObject);
        }
    }
}