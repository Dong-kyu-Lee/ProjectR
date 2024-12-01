using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PotionType
{   //포션의 종류
    HpIncrease,
    DamageIncrease,
    DamageReductionIncrease,
    CriticalPercentIncrease,
    CriticalDamageIncrease,
    AttackSpeedIncrease,
    MoveSpeedIncrease
}
public abstract class Potion : MonoBehaviour
{
    [SerializeField]
    private BasicItemData basicItemData;  //기본적인 아이템의 정보를 가지는 스크립터블 오브젝트
    [SerializeField]
    private PotionType type;

    public void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = basicItemData.ItemSprite;
    }

    public string ItemName { get { return basicItemData.ItemName; } }
    public string ItemDescription { get { return basicItemData.ItemName; } }
    public int ItemID { get { return basicItemData.ItemID; } }
    public Sprite Sprite { get { return basicItemData.ItemSprite; } }
    public PotionType Type { get { return type; } }

    //플레이어와 충돌 시 포션 사용
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DoEffect(other.GetComponent<PlayerStatus>());
            Destroy(gameObject);
        }
    }

    //각 포션들이 구현해야할 효과 발생 메소드
    public abstract void DoEffect(PlayerStatus playerStatus);
}
