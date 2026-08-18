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
                playerInventory = GameManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();
            }
        }

        // 인벤토리가 있다면 삭제 진행
        if (playerInventory != null)
        {
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

    // 장비 비교 기능 (상태 시뮬레이션 기반)

    public void AddCompareItem(EquipmentItemData clickedEquip, bool isEquipped)
    {
        if (clickedEquip == null) return;

        if (explainPanel != null && !explainPanel.activeSelf)
        {
            explainPanel.SetActive(true);
            isPanelActive = true;
        }

        // 1. 현재 인벤토리 장비칸 상태 확인
        bool hasEmptySlot = playerInventory != null && playerInventory.HasEmptyEquipmentSlot();

        if (hasEmptySlot)
        {
            // [상황 1] 빈 장비칸이 있는 경우: 기존 장비를 선택할 필요 없이 (0 + 새 장비)의 예상 스탯 즉시 출력
            if (!isEquipped) // 인벤토리에 있는 장비를 눌렀을 때만 작동
            {
                compareItem1 = null; // 빼야 할 기존 장비 없음
                compareItem2 = clickedEquip;

                UpdateCompareSlot(compareImage1, null); // 1번 슬롯(기존) 비우기
                UpdateCompareSlot(compareImage2, compareItem2.ItemSprite); // 2번 슬롯에 새 장비

                ShowComparisonResult();
            }
        }
        else
        {
            // [상황 2] 장비칸이 꽉 찬 경우: (기존 장비 + 새 장비) 두 가지를 모두 선택해야 시뮬레이션 진행
            if (isEquipped)
            {
                // 장비칸에 있는 아이템을 클릭하면 1번(기존) 슬롯에 등록
                compareItem1 = clickedEquip;
                UpdateCompareSlot(compareImage1, compareItem1.ItemSprite);
            }
            else
            {
                // 인벤토리에 있는 아이템을 클릭하면 2번(새 장비) 슬롯에 등록
                compareItem2 = clickedEquip;
                UpdateCompareSlot(compareImage2, compareItem2.ItemSprite);
            }

            // 두 개가 모두 선택되었을 때만 시뮬레이션 결과 출력
            if (compareItem1 != null && compareItem2 != null)
            {
                ShowComparisonResult();
            }
            else
            {
                if (compareResultText != null)
                {
                    compareResultText.text = "\n\n<color=#FFFF00>교체할 장착 장비와\n장착할 새 장비를\n모두 우클릭해주세요.</color>";
                }
            }
        }
    }

    private void UpdateCompareSlot(Image img, Sprite sprite)
    {
        if (img != null)
        {
            if (sprite != null)
            {
                img.sprite = sprite;
                img.color = Color.white;
            }
            else
            {
                img.color = transparentColor; // null이면 투명하게
            }
        }
    }

    private void ShowComparisonResult()
    {
        if (compareItem2 == null || compareResultText == null) return;

        string result = "";

        // 기존 장비(compareItem1)가 없으면 베이스 스탯을 0으로 두고 계산, 있으면 기존 스탯 반영
        float dam1 = compareItem1 != null ? compareItem1.Damage : 0f;
        float addDam1 = compareItem1 != null ? compareItem1.AdditionalDamage : 0f;
        float crit1 = compareItem1 != null ? compareItem1.CriticalPercent : 0f;
        float critDam1 = compareItem1 != null ? compareItem1.CriticalDamage : 0f;
        float dr1 = compareItem1 != null ? compareItem1.AdditionalDamageReduction : 0f;
        float atkSpd1 = compareItem1 != null ? compareItem1.AttackSpeed : 0f;
        float movSpd1 = compareItem1 != null ? compareItem1.AdditionalMoveSpeed : 0f;
        float price1 = compareItem1 != null ? compareItem1.PriceAdditional : 0f;
        float drIg1 = compareItem1 != null ? compareItem1.IgnoreDamageReduction : 0f;

        // 시뮬레이션 결과 계산 (새 장비 - 기존 장비)
        result += GetStatDiffText("공격력", dam1, compareItem2.Damage, false);
        result += GetStatDiffText("추가 피해량", addDam1, compareItem2.AdditionalDamage, true);
        result += GetStatDiffText("치명타 확률", crit1, compareItem2.CriticalPercent, true);
        result += GetStatDiffText("치명타 피해량", critDam1, compareItem2.CriticalDamage, true);
        result += GetStatDiffText("추가 피해 감소량", dr1, compareItem2.AdditionalDamageReduction, true);
        result += GetStatDiffText("공격 속도", atkSpd1, compareItem2.AttackSpeed, true);
        result += GetStatDiffText("이동 속도", movSpd1, compareItem2.AdditionalMoveSpeed, false);
        result += GetStatDiffText("재화 획득량", price1, compareItem2.PriceAdditional, false);
        result += GetStatDiffText("피해 감소 무시", drIg1, compareItem2.IgnoreDamageReduction, true);

        if (string.IsNullOrEmpty(result))
        {
            result = "스탯 변화 없음";
        }

        compareResultText.text = result;
    }

    private string GetStatDiffText(string statName, float val1, float val2, bool isPercent)
    {
        float diff = val2 - val1;
        if (Mathf.Abs(diff) < 0.001f) return "";

        string sign = diff > 0 ? "+" : "";
        string colorHex = diff > 0 ? "#00FF00" : "#FF0000";

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