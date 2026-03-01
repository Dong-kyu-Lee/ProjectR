using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    [SerializeField]
    private BasicItemData myItemData;                   //기본적인 아이템의 정보를 가지는 스크립터블 오브젝트
    [SerializeField]
    private int amount = 1;                             //습득했을 경우 얻게되는 아이템 수
    private PlayerItemUseController playerController;   //플레이어의 아이템 사용 컨트롤러

    public BasicItemData MyItemData { get { return myItemData; } }
    public int Amount { get { return amount; } }

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = myItemData.ItemSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<PlayerItemUseController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = null;
        }
    }
}
