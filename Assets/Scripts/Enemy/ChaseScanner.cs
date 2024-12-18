using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseScanner : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = transform.parent.GetComponent<Enemy>();
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
            enemy.SetTarget(collision.transform);
            gameObject.SetActive(false);
        }
    }
}
