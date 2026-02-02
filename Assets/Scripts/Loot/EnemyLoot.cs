using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;

public class EnemyLoot : MonoBehaviour
{
    public LootTable lootTable;
    private PlayerStatus playerStatus;

    private void Start()
    {
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
    }

    // 루트테이블 아이템 드롭
    public void DropLoot()
    {
        if (lootTable == null) return;

        foreach (var drop in lootTable.dropItems)
        {
            for (int i = 0; i < drop.maxCount; i++)
            {
                if (Random.value <= drop.dropRate)
                {
                    DropItem(drop);
                    BonusDrop(drop);
                }
            }
        }
    }

    // 랜덤 위치에 아이템 드롭
    private void DropItem(DropItem drop)
    {
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0);
        GameObject dropObj = Instantiate(drop.prefab, spawnPos, Quaternion.identity);

        Rigidbody2D rigid = dropObj.GetComponent<Rigidbody2D>();
        if (rigid != null)
        {
            rigid.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(3f, 5f)), ForceMode2D.Impulse);
        }
    }

    // 재화 획득량에 따른 보너스 아이템 드롭
    private void BonusDrop(DropItem drop)
    {
        float addPrice = playerStatus.PriceAdditional;

        // 재화 획득량이 100% 이상이면 확정 획득
        int guaranteedCount = Mathf.FloorToInt(addPrice);
        for (int i = 0; i < guaranteedCount; i++)
        {
            DropItem(drop);
        }

        // 100% 미만이면 확률적으로 획득
        float extraChance = addPrice - guaranteedCount;
        if (Random.value <= extraChance)
        {
            DropItem(drop);
        }
    }
}
