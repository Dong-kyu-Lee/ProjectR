using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteProjectile : Projectile
{
    [SerializeField]
    private float timeToExplode = 5.0f;     //폭발 시간
    [SerializeField]
    private float explosionDmg = 100.0f;    //폭발 데미지


    private void Start()
    {
        StartCoroutine(StartExplodeTimer());
    }

    private IEnumerator StartExplodeTimer()
    {
        yield return new WaitForSeconds(timeToExplode);
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        Debug.Log("폭발함");
        ProcessExplosionDamage();
    }

    //폭발 데미지 처리 함수
    private void ProcessExplosionDamage()  
    {
        Collider2D[] mops = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D mop in mops)
        {
            if(mop.CompareTag("Player"))
            {
                mop.GetComponent<PlayerStatus>().Hp -= explosionDmg;
            }
        }

    }

}
