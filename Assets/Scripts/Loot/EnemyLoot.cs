using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    public LootTable lootTable;

    public void DropLoot()
    {
        if (lootTable == null) return;

        foreach (var drop in lootTable.dropItems)
        {
            for (int i = 0; i < drop.maxCount; i++)
            {
                if (Random.value <= drop.dropRate)
                {
                    Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0);
                    GameObject dropObj = Instantiate(drop.prefab, spawnPos, Quaternion.identity);

                    Rigidbody2D rigid = dropObj.GetComponent<Rigidbody2D>();
                    if (rigid != null)
                    {
                        rigid.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(3f, 5f)), ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
}
