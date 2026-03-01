using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("UI 및 데이터 참조")]
    [SerializeField]
    private SpriteRenderer[] itemImage;
    [SerializeField]
    private BasicItemData[] item; // 현재 슬롯에 있는 아이템들
    [SerializeField]
    private ItemExplain[] itemExplain;

    [Header("설정")]
    [SerializeField]
    private DropableItem dropableItem; // 확률 및 아이템 풀 관리자
    [SerializeField]
    private BasicItemData noneItem;    // 빈 슬롯용 아이템

    [Header("플레이어 참조")]
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private PlayerStatus playerStatus;

    private void Awake()
    {
        if (inventory == null)
            inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<Inventory>();

        if (playerStatus == null)
            playerStatus = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStatus>();

        // 시작 시 상점 물건 진열
        SellingItem();
    }

    // 상점 물건 갱신
    public void SellingItem()
    {
        // 이미 가진 장비 중복 방지용
        List<BasicItemData> ownedItems = null;
        if (inventory != null)
            ownedItems = inventory.GetOwnedItems();

        List<BasicItemData> newStock = dropableItem.GetShopItems(ownedItems);

        // 상점 슬롯에 배치
        for (int i = 0; i < item.Length; i++)
        {
            if (i < newStock.Count)
            {
                // 아이템이 있으면 채워넣기
                item[i] = newStock[i];
                itemImage[i].sprite = newStock[i].ItemSprite;
                itemImage[i].color = Color.white; // 투명도나 색상 초기화
                if (itemExplain[i] != null) itemExplain[i].item = newStock[i];
            }
            else
            {
                // 아이템이 모자라거나 슬롯이 남으면 빈 슬롯 처리
                SetSlotEmpty(i);
            }
        }
    }

    // 슬롯을 비우는 함수
    private void SetSlotEmpty(int index)
    {
        item[index] = noneItem;
        itemImage[index].sprite = null;

        if (itemExplain[index] != null) itemExplain[index].item = noneItem;
    }

    public void EmptySlot(BasicItemData itemB)
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] == itemB)
            {
                SetSlotEmpty(i);
            }
        }
    }

    public void BuyItem(BasicItemData targetItem)
    {
        // noneItem을 사려고 하면 무시
        if (targetItem == noneItem) return;

        if (targetItem.ItemPrice <= playerStatus.Gold)
        {
            playerStatus.Gold -= targetItem.ItemPrice;

            inventory.AddItem(targetItem);

            // 상점 슬롯에서 비우기
            EmptySlot(targetItem);
        }
        else
        {
            Debug.Log("골드가 부족합니다.");
        }
    }
}