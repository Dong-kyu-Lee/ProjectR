using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D hitBoxCol;

    [SerializeField]
    private float damage;

    bool isHit;

    private void Awake()
    {
        damage = gameObject.GetComponentInParent<EnemyStatus>().Damage;
    }


    void Start()
    {
        StartCoroutine(HitBoxCoroutine());
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        isHit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!isHit)
            {
                isHit = true;
                collision.gameObject.GetComponent<Status>().TakeDamage(damage, 0);
            }
        }
        gameObject.SetActive(false);
    }

    IEnumerator HitBoxCoroutine()
    {
        if (gameObject.activeSelf == true)
        {
            yield return new WaitForSeconds(0.1f);

            gameObject.SetActive(false);
        }
    }
}
