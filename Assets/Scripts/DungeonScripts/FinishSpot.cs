using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpot : MonoBehaviour
{
    private void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            DungeonFlowManager.Instance.LoadNextDungeon();
        }
    }
}
