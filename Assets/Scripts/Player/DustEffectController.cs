using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustEffectController : MonoBehaviour
{
    public float delay = 0.5f;

    private void Start()
    {
        Destroy(gameObject, delay);
    }
}
