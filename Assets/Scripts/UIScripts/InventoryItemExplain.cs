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

    // 인벤토리(이 객체가 포함된 상위 오브젝트)가 켜질 때 실행
    void OnEnable()
    {
        // 인벤토리가 열리면 설명창 패널도 강제로 활성화
        if (explainPanel != null)
            explainPanel.SetActive(true);

        isPanelActive = true;

        // 처음 열렸을 때는 선택된 아이템이 없으므로 빈 상태로 초기화
        ClearPanel();
    }

    void OnDisable()
    {
        isPanelActive = false;
    }

    public void OnItemSlotClicked(BasicItemData itemData)
    {
        //Dummy 아이템이면 무시 -> 무시하는 대신 내용을 비워줌
        if (itemData == null || itemData.ItemName == "Dummy")
        {
            ClearPanel();
            return;
        }

        // 패널은 이미 켜져 있으므로 내용만 갱신
        UpdatePanel(itemData);
    }

    public void ShowPanel(BasicItemData itemData)
    {
        explainPanel.SetActive(true);
        UpdatePanel(itemData);
        isPanelActive = true;
    }

    private void UpdatePanel(BasicItemData itemData)
    {
        currentItemData = itemData;

        // 아이템이 선택되면 이미지를 다시 보이게 설정
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemData.ItemSprite;
        }

        if (itemNameText != null)
            itemNameText.text = itemData.ItemName;
        if (itemEffectText != null)
            itemEffectText.text = itemData.ItemExplain;
        if (itemDescriptionText != null)
            itemDescriptionText.text = itemData.ItemDescription;
    }

    // 패널을 빈 상태로 초기화하는 함수
    private void ClearPanel()
    {
        currentItemData = null;

        // 이미지는 숨김 (빈 하얀 네모 방지)
        if (itemImage != null)
            itemImage.gameObject.SetActive(false);

        if (itemNameText != null)
            itemNameText.text = "";
        if (itemEffectText != null)
            itemEffectText.text = "";

        // 안내 문구 표시
        if (itemDescriptionText != null)
            itemDescriptionText.text = " ";
    }

    public void ClosePanel()
    {
        explainPanel.SetActive(false);
        isPanelActive = false;

        //닫힌 UI 스택에서 제거
        InGameUIManager.Instance.UnregisterUI(explainPanel);
    }
}