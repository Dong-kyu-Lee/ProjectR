using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    private static UpgradeUI Instance;
    private GameObject upgradeStatusUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
            return;
        }

        upgradeStatusUI = transform.GetChild(0).gameObject;
    }

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
