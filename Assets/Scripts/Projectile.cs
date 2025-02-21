using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D projectileRigidbody;
    public PlayerStatus playerStatus;
    public float damage;
    public float ignoreDamageReduction;

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
        float damage = playerStatus.TotalDamage;
        float ignoreDamageReduction = playerStatus.IgnoreDamageReduction;
        bool isCritical = false;

        if (collision.transform.tag == "Enemy")
        {
            damage = CalcDamage.Instance.CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
            collision.gameObject.GetComponent<Status>().TakeDamage(damage, ignoreDamageReduction, isCritical);
            CalcDamage.Instance.CheckAddtionalDamage(collision.gameObject);
            CalcDamage.Instance.AdditionalEffect(collision.gameObject);
            CalcDamage.Instance.CheckFightState();
        }
        gameObject.SetActive(false);
    }
}
