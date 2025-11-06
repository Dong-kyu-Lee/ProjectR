using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemExplain : MonoBehaviour
{
    [Header("참조 설정")]
    [SerializeField] private GameObject explainPanel;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemEffectText;
    [SerializeField] private Text itemDescriptionText;

    private BasicItemData currentItemData;
    private bool isPanelActive = false;

    void Start()
    {
        if (explainPanel != null)
            explainPanel.SetActive(false);
    }

    void Update()
    {
        //Esc 키로 설명창 닫기
        if (isPanelActive && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    public void OnItemSlotClicked(BasicItemData itemData)
    {
        //Dummy 아이템이면 무시
        if (itemData.ItemName == "Dummy")
            return;

        if (!isPanelActive)
        {
            ShowPanel(itemData);
        }
        else
        {
            if (itemData != currentItemData)
                UpdatePanel(itemData);
        }
    }

    public void ShowPanel(BasicItemData itemData)
    {
        explainPanel.SetActive(true);
        UpdatePanel(itemData);
        isPanelActive = true;

        //열린 UI 등록
        InGameUIManager.Instance.RegisterUI(explainPanel);
    }

    private void UpdatePanel(BasicItemData itemData)
    {
        currentItemData = itemData;

        if (itemImage != null)
            itemImage.sprite = itemData.ItemSprite;
        if (itemNameText != null)
            itemNameText.text = itemData.ItemName;
        if (itemEffectText != null)
            itemEffectText.text = itemData.ItemExplain;
        if (itemDescriptionText != null)
            itemDescriptionText.text = itemData.ItemDescription;
    }

    public void ClosePanel()
    {
        explainPanel.SetActive(false);
        isPanelActive = false;

        //닫힌 UI 스택에서 제거
        InGameUIManager.Instance.UnregisterUI(explainPanel);
    }
}
