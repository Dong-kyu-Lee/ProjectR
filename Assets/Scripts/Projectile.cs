using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D projectileRigidbody;
    public float damage;

    Vector2 velocity;

    // 발사 속도 프로퍼티
    public Vector2 Velocity
    {
        get
        {
            return velocity;
        }
        set
        {
            velocity = value;
            projectileRigidbody.velocity = velocity * 50f;
        }
    }

    void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Status>().TakeDamage(damage);
        }
        gameObject.SetActive(false);
    }
}
