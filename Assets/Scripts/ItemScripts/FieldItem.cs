using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    [SerializeField]
    //private ConsumableItemData myItemData;  //필드에 떨어진 아이템의 정보
    private BasicItemData myItemData;  //기본적인 아이템의 정보를 가지는 스크립터블 오브젝트

    [SerializeField]
    private int amount = 1;                 //습득했을 경우 얻게되는 아이템 수
    private PlayerItemUseController playerController = null;    //플레이어의 아이템 사용 컨트롤러
    [SerializeField]
    private ItemDescriptionPanel itemDescriptionPanel;   //아이템 설명 패널

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = myItemData.ItemSprite;
        itemDescriptionPanel = transform.GetChild(0).GetComponent<ItemDescriptionPanel>();
        InitItemDescriptionPanel();
    }

    private void Update()
    {
        if (playerController != null && Input.GetKeyDown(KeyCode.E))
        {
            AddToPlayerInventory(playerController);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //아이템 설명 활성화
            Debug.Log("아이템 설명 활성화");
            itemDescriptionPanel.gameObject.SetActive(true);
            playerController = collision.GetComponent<PlayerItemUseController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //아이템 설명 비활성화
            Debug.Log("아이템 설명 비활성화");
            itemDescriptionPanel.gameObject.SetActive(false);
            playerController = null;
        }
    }

    //자신의 아이템 정보 및 수량을 플레이어의 인벤토리에 넣는 메서드
    private void AddToPlayerInventory(PlayerItemUseController itemUseController)
    {
        if(itemUseController.AddItem(myItemData, amount))
        {
            myItemData = null;
            Destroy(gameObject);
        }
    }

    //자신의 아이템 설명 패널을 초기화하는 메서드
    private void InitItemDescriptionPanel()
    {
        itemDescriptionPanel.ItemName = myItemData.ItemName;
        itemDescriptionPanel.ItemDescription = myItemData.ItemDescription;
    }
}
