using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{

    void Start()
    {
        
    }

    void LateUpdate()
    {
        Vector3 newPosition = GameManager.Instance.CurrentPlayer.transform.position;
        transform.position = newPosition + Vector3.back*7.0f;
    }
}
