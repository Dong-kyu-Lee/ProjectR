using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGImageMove : MonoBehaviour
{
    [Header("Background Image Settings")]
    [SerializeField]
    private GameObject[] backgroundImageLayer;

    [Tooltip("배경 이미지 중심 좌표")]
    [SerializeField]
    private Vector3 backgroundPosition; 

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
