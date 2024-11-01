using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D projectileRigidbody;

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
}
