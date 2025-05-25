using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class LiberationUI : MonoBehaviour
{
    [SerializeField] private GameObject liberationUI;
    private bool isOpen;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!isOpen)
            {
                liberationUI.SetActive(true);
                isOpen = true;
            }
            else
            {
                liberationUI.SetActive(false);
                isOpen = false;
            }
        }
    }
}
