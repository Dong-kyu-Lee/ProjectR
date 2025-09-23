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
            if (liberationUI == null)
            {
                Debug.LogError("liberationUI is null!");
                return;
            }

            isOpen = !isOpen;
            liberationUI.SetActive(isOpen);
        }
    }
}
