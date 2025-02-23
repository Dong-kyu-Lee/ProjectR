using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMonsterScript : MonoBehaviour
{
    
    public void Die()
    {
        gameObject.SetActive(false);
    }
}
