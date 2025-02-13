using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerativeGrenade : Grenade
{
    [SerializeField]
    private GameObject generatedPrefab; // 생성할 대상 프리팹

    protected override void Explode()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
        if (hit)
        {
            Instantiate(generatedPrefab, hit.point, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
