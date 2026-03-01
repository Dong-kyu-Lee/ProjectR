using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 상점 데이터 구조 (확률 + 아이템 목록 직접 관리)

[System.Serializable]
public class ShopTierData
{
    public ItemGrade grade;

    [Tooltip("등장 확률 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    public float probability;

    public List<BasicItemData> manualItemList;
}

#endregion

public class DropableItem : MonoBehaviour
{
    [Tooltip("상점에 진열할 총 아이템 슬롯 개수")]
    public int shopSlotCount = 4; // 기본값 4칸

    [Header("상점 구성 데이터")]
    [Tooltip("등급별 확률과 아이템 목록을 설정하세요.")]
    public List<ShopTierData> shopTiers;

    private void Reset()
    {
        shopTiers = new List<ShopTierData>();
        foreach (ItemGrade g in System.Enum.GetValues(typeof(ItemGrade)))
        {
            shopTiers.Add(new ShopTierData
            {
                grade = g,
                probability = 0.2f,
                manualItemList = new List<BasicItemData>() // 빈 리스트 생성
            });
        }
    }

    public List<BasicItemData> GetShopItems(List<BasicItemData> ownedItems = null)
    {
        return GetRandomStock(shopSlotCount, ownedItems);
    }

    public List<BasicItemData> GetRandomStock(int count, List<BasicItemData> ownedItems = null)
    {
        List<BasicItemData> resultStock = new List<BasicItemData>();
        HashSet<BasicItemData> currentStockSet = new HashSet<BasicItemData>();

        // 플레이어 소유 아이템 Set 
        HashSet<BasicItemData> ownedSet = (ownedItems != null)
            ? new HashSet<BasicItemData>(ownedItems)
            : new HashSet<BasicItemData>();

        int safetyLoop = 0; // 무한루프 방지

        while (resultStock.Count < count && safetyLoop < 100)
        {
            safetyLoop++;

            // 등급 선택
            ShopTierData selectedTier = GetRandomTier();

            // 리스트가 비어있으면 다시 뽑기
            if (selectedTier == null || selectedTier.manualItemList.Count == 0)
                continue;

            // 리스트 내에서 아이템 랜덤 선택
            BasicItemData candidate = selectedTier.manualItemList[Random.Range(0, selectedTier.manualItemList.Count)];

            // 중복 방지
            if (currentStockSet.Contains(candidate)) continue;

            // 소유 여부 체크
            if (ownedSet.Contains(candidate)) continue;

            // 확정 및 추가
            resultStock.Add(candidate);
            currentStockSet.Add(candidate);
        }

        return resultStock;
    }

    private ShopTierData GetRandomTier()
    {
        if (shopTiers == null || shopTiers.Count == 0) return null;

        float totalProb = 0f;
        foreach (var tier in shopTiers) totalProb += tier.probability;

        float rand = Random.value * totalProb;
        float cumulative = 0f;

        foreach (var tier in shopTiers)
        {
            cumulative += tier.probability;
            if (rand <= cumulative)
            {
                return tier;
            }
        }

        // 계산 오류 시 첫 번째 항목 반환
        return shopTiers[0];
    }
}