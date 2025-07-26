using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffHitBox : HitBox
{
    [SerializeField] private float slowAmount = 0.3f;
    [SerializeField] private float slowDuration = 3f;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isHit)
        {
            isHit = true;
            collision.GetComponent<Status>().TakeDamage(enemy, damage, 0, false);
            collision.GetComponent<PlayerStatus>().ApplySlow(slowAmount, slowDuration);
        }

        gameObject.SetActive(false);
    }
}
