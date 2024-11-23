using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteProjectile : Projectile
{
    [SerializeField]
    private float timeToExplode = 5.0f;

    private void Start()
    {
        //일부로 빈 메서드 정의함. 추후 논의 필요
    }

    private void Update()
    {
        if (0.0f <= timeToExplode)
            timeToExplode -= Time.deltaTime;
        else
        {
            Explode();
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        Debug.Log("폭발함");
    }
}
