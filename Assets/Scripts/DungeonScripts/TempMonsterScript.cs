using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMonsterScript : MonoBehaviour
{
    public event Action<TempMonsterScript> onDeath;
    
    public void Die()
    {
        onDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}
