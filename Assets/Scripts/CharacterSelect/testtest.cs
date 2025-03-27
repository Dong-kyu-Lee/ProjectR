using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtest : MonoBehaviour
{
    GameObject currentPlayer;
    void Start()
    {
        currentPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/Player1_2"));
        GameManager.Instance.CurrentPlayer = currentPlayer;
    }
}
