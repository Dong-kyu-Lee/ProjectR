using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableStatusUI : MonoBehaviour
{
    [SerializeField] private GameObject upgradeStatusUI;

    void Update()
    {
        SetActiveUI();
    }

    public void SetActiveUI()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (upgradeStatusUI != null)
            {
                bool isActive = upgradeStatusUI.activeSelf;
                upgradeStatusUI.SetActive(!isActive);
            }
        }
    }
}
