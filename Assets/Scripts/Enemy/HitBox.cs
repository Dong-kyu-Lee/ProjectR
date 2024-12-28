using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D hitBoxCol;

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
            Debug.Log("hit");

            gameObject.SetActive(false);
        }
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
