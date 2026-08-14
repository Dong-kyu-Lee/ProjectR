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

    [Header("비교 UI 설정")]
    [SerializeField] private Image compareImage1;
    [SerializeField] private Image compareImage2;
    [SerializeField] private Text compareResultText; // StatusChangeText 안의 ItemDescriptionTxt 연결

    private BasicItemData currentItemData;
    private bool isPanelActive = false;
    private Inventory playerInventory;

    // 비교용 상태 변수
    private EquipmentItemData compareItem1;
    private EquipmentItemData compareItem2;
    private readonly Color transparentColor = new Color(1, 1, 1, 0); // 빈 슬롯용 투명 색상

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
        ClearCompareData(); // 패널이 닫힐 때 비교 데이터 완전 초기화
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
                // 최상위가 아닌 자식 오브젝트에서 안전하게 Inventory 탐색
                playerInventory = GameManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();
            }
        }

        // 인벤토리가 있다면 삭제 진행
        if (playerInventory != null)
        {
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
    }

    // ==========================================
    // 장비 비교 기능 관련 메서드
    // ==========================================

    public void AddCompareItem(EquipmentItemData newEquip)
    {
        if (newEquip == null) return;
        Debug.Log($"[디버그] AddCompareItem 실행됨: {newEquip.ItemName}");

        // 우클릭했을 때 패널이 꺼져 있다면 강제로 켭니다.
        if (explainPanel != null && !explainPanel.activeSelf)
        {
            explainPanel.SetActive(true);
            isPanelActive = true;
        }

        if (compareItem1 == null)
        {
            Debug.Log("[디버그] 1번 비교 슬롯 등록 완료");
            compareItem1 = newEquip;
            UpdateCompareSlot(compareImage1, compareItem1.ItemSprite);
        }
        else if (compareItem2 == null)
        {
            Debug.Log("[디버그] 2번 비교 슬롯 등록 및 스탯 비교 완료");
            compareItem2 = newEquip;
            UpdateCompareSlot(compareImage2, compareItem2.ItemSprite);
            ShowComparisonResult();
        }
        else
        {
            Debug.Log("[디버그] 슬롯 밀어내기 및 2번 비교 슬롯 갱신 완료");
            compareItem1 = compareItem2;
            UpdateCompareSlot(compareImage1, compareItem1.ItemSprite);

            compareItem2 = newEquip;
            UpdateCompareSlot(compareImage2, compareItem2.ItemSprite);
            ShowComparisonResult();
        }
    }

    private void UpdateCompareSlot(Image img, Sprite sprite)
    {
        if (img != null)
        {
            img.sprite = sprite;
            img.color = Color.white; // 투명도 복구
        }
    }

    private void ShowComparisonResult()
    {
        if (compareItem1 == null || compareItem2 == null || compareResultText == null) return;

        string result = "";

        // 1번 슬롯(기존)에서 2번 슬롯(새 장비)으로 넘어갈 때의 변화량 계산
        result += GetStatDiffText("공격력", compareItem1.Damage, compareItem2.Damage, false);
        result += GetStatDiffText("추가 피해량", compareItem1.AdditionalDamage, compareItem2.AdditionalDamage, true);
        result += GetStatDiffText("치명타 확률", compareItem1.CriticalPercent, compareItem2.CriticalPercent, true);
        result += GetStatDiffText("치명타 피해량", compareItem1.CriticalDamage, compareItem2.CriticalDamage, true);
        result += GetStatDiffText("추가 피해 감소량", compareItem1.AdditionalDamageReduction, compareItem2.AdditionalDamageReduction, true);
        result += GetStatDiffText("공격 속도", compareItem1.AttackSpeed, compareItem2.AttackSpeed, true);
        result += GetStatDiffText("이동 속도", compareItem1.AdditionalMoveSpeed, compareItem2.AdditionalMoveSpeed, false);
        result += GetStatDiffText("재화 획득량", compareItem1.PriceAdditional, compareItem2.PriceAdditional, false);
        result += GetStatDiffText("피해 감소 무시", compareItem1.IgnoreDamageReduction, compareItem2.IgnoreDamageReduction, true);

        if (string.IsNullOrEmpty(result))
        {
            result = "스탯 차이 없음";
        }

        compareResultText.text = result;
    }

    private string GetStatDiffText(string statName, float val1, float val2, bool isPercent)
    {
        float diff = val2 - val1;
        if (Mathf.Abs(diff) < 0.001f) return ""; // 변화가 없으면 출력하지 않음

        string sign = diff > 0 ? "+" : "";
        string colorHex = diff > 0 ? "#00FF00" : "#FF0000"; // 오르면 초록색, 내리면 빨간색

        // 퍼센트 스탯이면 100을 곱해서 %를 붙여주고, 고정 스탯은 그대로 출력
        string displayValue = isPercent ? $"{diff * 100f:0.##}%" : $"{diff:0.##}";

        return $"{statName}: <color={colorHex}>{sign}{displayValue}</color>\n";
    }

    public void ClearCompareData()
    {
        compareItem1 = null;
        compareItem2 = null;

        if (compareImage1 != null) compareImage1.color = transparentColor;
        if (compareImage2 != null) compareImage2.color = transparentColor;
        if (compareResultText != null) compareResultText.text = "";
    }
}