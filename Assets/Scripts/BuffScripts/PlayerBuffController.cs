using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;


public class PlayerBuffController : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus playerStatus;
    
    private void Awake()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }

}
