using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class LiberationUI : MonoBehaviour
{
    [SerializeField] private GameObject liberationUI;
    private bool isOpen;
    private void Awake()
    {
        liberationUI.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (liberationUI == null)
            {
                Debug.LogError("liberationUI is null!");
                return;
            }
            CloseUI();
        }
    }
    public void OpenUI()
    {
        liberationUI.SetActive(true);
    }
    public void CloseUI()
    {
        liberationUI.SetActive(false);
    }
}
