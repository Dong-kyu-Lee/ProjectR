using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItemGradeChance
{
    public ItemGrade grade;
    [Range(0f, 1f)]
    public float probability;
}


public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] itemImage;
    [SerializeField]
    private BasicItemData[] item;
    [SerializeField]
    private DropableItem dropableItem;
    [SerializeField]
    private BasicItemData noneItem;
    [SerializeField]
    private ItemExplain[] itemExplain;
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private PlayerStatus playerStatus;
    bool isOverlap;

    [Header("등급별 등장 확률")]
    [SerializeField]
    private List<ShopItemGradeChance> itemGradeChances;

    private void Awake()
    {
        if (inventory == null)
            inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<Inventory>();

        if (playerStatus == null)
            playerStatus = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStatus>();

        SellingItem();
    }


    public void SellingItem()
    {
        int[] checkOverlap = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (item[i] != noneItem)
            {
                do
                {
                    isOverlap = false;
                    // 1. 등급 먼저 확률로 결정
                    ItemGrade selectedGrade = GetRandomGradeByProbability();

                    // 2. 해당 등급의 아이템 후보들 필터링
                    List<BasicItemData> candidates = dropableItem.dropableItem.FindAll(
                        item => item.ItemGrade == selectedGrade
                    );

                    if (candidates.Count == 0)
                    {
                        Debug.LogWarning($"[ShopManager] {selectedGrade} 등급 아이템이 없습니다.");
                        break;
                    }

                    // 3. 중복 방지를 위해 랜덤 인덱스 반복
                    int randomIndex = Random.Range(0, candidates.Count);
                    BasicItemData selectedItem = candidates[randomIndex];

                    // 중복 체크
                    for (int j = 0; j < i; j++)
                    {
                        if (item[j] == selectedItem)
                        {
                            isOverlap = true;
                            break;
                        }
                    }

                    if (!isOverlap)
                    {
                        item[i] = selectedItem;
                        itemImage[i].sprite = selectedItem.ItemSprite;
                        itemExplain[i].item = selectedItem;
                    }

                } while (isOverlap);
            }
        }
    }
    private ItemGrade GetRandomGradeByProbability()
    {
        float rand = Random.value;
        float cumulative = 0f;

        foreach (var chance in itemGradeChances)
        {
            cumulative += chance.probability;
            if (rand <= cumulative)
            {
                return chance.grade;
            }
        }

        // 기본값
        return ItemGrade.Normal;
    }


    public void EmptySlot(BasicItemData itemB)
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] == itemB)
            {
                item[i] = noneItem;
                itemImage[i].sprite = null;
                itemExplain[i].item = noneItem;
            }
        }
    }

    public void BuyItem(BasicItemData item)
    {
        if (item.ItemPrice <= playerStatus.Gold)
        {
            playerStatus.Gold -= item.ItemPrice;
            EmptySlot(item);
            inventory.AddItem(item);
            dropableItem.removeItem(item);
        }
        else
            Debug.Log("골드가 부족합니다.");
    }
}
