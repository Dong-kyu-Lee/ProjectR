using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static PlayerObj;

public class LiberationUI : MonoBehaviour
{
    [SerializeField] private GameObject liberationUI;
    [SerializeField] private LiberationSystem liberationSystem;

    private void Awake()
    {
        liberationUI.SetActive(false);
    }

    private void OnEnable()
    {
        liberationSystem.SyncSteadfiteText();
        SaveManager.Instance.SyncFromLiberationData();
    }

    public void CloseUIOnclick()
    {
        if (liberationUI != null) liberationUI.SetActive(false);
    }
}
