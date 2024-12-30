using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D hitBoxCol;

    [SerializeField]
    private float damage;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Status>().TakeDamage(damage);
        }
        gameObject.SetActive(false);
    }

    IEnumerator HitBoxCoroutine()
    {
        if (gameObject.activeSelf == true)
        {
            yield return new WaitForSeconds(0.5f);

            gameObject.SetActive(false);
        }
    }
}
