using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance { get; private set; }
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

    // 외부(인벤토리 버튼 등)에서 스탯 창을 강제로 열어주는 함수
    public void OpenUI()
    {
        if (upgradeStatusUI != null)
        {
            upgradeStatusUI.SetActive(true);
        }
    }

    public void CloseButton()
    {
        upgradeStatusUI.SetActive(false);
    }
}