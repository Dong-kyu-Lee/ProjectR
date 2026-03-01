using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : MonoBehaviour
{
    [SerializeField] private float duration = 2f;

    private void Awake()
    {
        Destroy(gameObject, duration);
    }
}
