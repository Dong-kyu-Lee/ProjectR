using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScanner : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = transform.parent.GetComponent<Enemy>();
        gameObject.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            enemy.StartAttack();
            gameObject.SetActive(false);
        }
    }

    public void ActivateScanner()
    {
        gameObject.SetActive(true);
    }
}
