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
    [SerializeField] private Text itemGradeText;

    [Header("기능 버튼")]
    [SerializeField] private Button discardButton;

    private BasicItemData currentItemData;
    private bool isPanelActive = false;
    private Inventory playerInventory;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            playerInventory = GameManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();
        }
        else
        {
            Debug.LogWarning("InventoryItemExplain: GameManager 또는 CurrentPlayer를 찾을 수 없습니다.");
        }

        // 버튼 리스너 연결
        if (discardButton != null)
        {
            discardButton.onClick.AddListener(OnClickDiscard);
            discardButton.interactable = false;
        }
    }

    void OnEnable()
    {
        if (explainPanel != null)
            explainPanel.SetActive(true);

        isPanelActive = true;
        ClearPanel();
    }

    void OnDisable()
    {
        isPanelActive = false;
    }

    public void OnItemSlotClicked(BasicItemData itemData)
    {
        if (itemData == null || itemData.ItemName == "Dummy")
        {
            ClearPanel();
            return;
        }
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

        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemData.ItemSprite;
        }

        if (itemNameText != null) itemNameText.text = itemData.ItemName;
        if (itemEffectText != null) itemEffectText.text = itemData.ItemExplain;
        if (itemDescriptionText != null) itemDescriptionText.text = itemData.ItemDescription;

        if (itemGradeText != null)
        {
            if (itemData.ItemGrade == ItemGrade.Dummy)
            {
                itemGradeText.text = "";
            }
            else
            {
                itemGradeText.text = itemData.ItemGrade.ToString();
            }
        }
       if (discardButton != null) discardButton.interactable = true;
    }

    private void ClearPanel()
    {
        currentItemData = null;

        if (itemImage != null) itemImage.gameObject.SetActive(false);
        if (itemNameText != null) itemNameText.text = "";
        if (itemEffectText != null) itemEffectText.text = "";
        if (itemDescriptionText != null) itemDescriptionText.text = " ";
        if (itemGradeText != null)
        {
            itemGradeText.text = "";
        }
        if (discardButton != null) discardButton.interactable = false;
    }

    public void OnClickDiscard()
    {
        if (currentItemData == null) return;

        // 인벤토리 참조 재확인
        if (playerInventory == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
            {
                playerInventory = GameManager.Instance.CurrentPlayer.GetComponent<Inventory>();
            }
        }

        // 인벤토리가 있다면 삭제 진행
        if (playerInventory != null)
        {
            // 장비라면 장착 해제 먼저
            if (currentItemData is EquipmentItemData equipData)
            {
                PlayerStatus status = playerInventory.GetComponent<PlayerStatus>();
                if (status != null) equipData.UnEquipItem(status);
            }

            // 인벤토리 리스트에서 삭제 요청
            playerInventory.RemoveItem(currentItemData);

            Debug.Log($"{currentItemData.ItemName}을(를) 버렸습니다.");
            ClearPanel();
        }
        else
        {
            Debug.LogError("플레이어 인벤토리를 찾을 수 없어 아이템을 버리지 못했습니다.");
        }
    }

    public void ClosePanel()
    {
        explainPanel.SetActive(false);
        isPanelActive = false;
        InGameUIManager.Instance.UnregisterUI(explainPanel);
    }
}