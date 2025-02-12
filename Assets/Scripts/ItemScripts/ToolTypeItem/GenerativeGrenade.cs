using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerativeGrenade : Grenade
{
    [SerializeField]
    private GameObject generatedPrefab; // 생성할 대상 프리팹

    protected override void Explode()
    {
        Instantiate(generatedPrefab, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
