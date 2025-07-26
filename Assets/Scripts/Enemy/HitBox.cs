using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    protected BoxCollider2D hitBoxCol;
    [SerializeField]
    protected float damage;

    protected GameObject enemy;

    protected bool isHit;

    private void Awake()
    {
        enemy = gameObject.transform.parent.gameObject;
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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isHit)
        {
            isHit = true;
            collision.gameObject.GetComponent<Status>().TakeDamage(enemy, damage, 0, false);
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
