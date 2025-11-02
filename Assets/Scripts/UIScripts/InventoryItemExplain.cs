using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemExplain : MonoBehaviour
{
    [Header("참조 설정")]
    [SerializeField] private GameObject explainPanel;       //설명창 UI 패널
    [SerializeField] private Image itemImage;               //아이템 이미지
    [SerializeField] private Text itemNameText;             //아이템 이름
    [SerializeField] private Text itemEffectText;           //아이템 효과
    [SerializeField] private Text itemDescriptionText;      //아이템 설정

    private BasicItemData currentItemData;                  //현재 설명 중인 아이템 데이터
    private bool isPanelActive = false;                     //패널 활성화 여부

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

    //아이템 슬롯 클릭 시 호출
    public void OnItemSlotClicked(BasicItemData itemData)
    {
        //Dummy 아이템이면 무시
        if (itemData.ItemName == "Dummy" )
            return;

        //패널이 꺼져 있다면 활성화
        if (!isPanelActive)
        {
            ShowPanel(itemData);
        }
        else
        {
            //패널이 켜져 있을 경우, 다른 아이템이면 갱신
            if (itemData != currentItemData)
                UpdatePanel(itemData);
        }
    }

    //설명창 표시
    public void ShowPanel(BasicItemData itemData)
    {
        explainPanel.SetActive(true);
        UpdatePanel(itemData);
        isPanelActive = true;
    }

    //설명창 갱신
    private void UpdatePanel(BasicItemData itemData)
    {
        currentItemData = itemData;

        if (itemImage != null)
            itemImage.sprite = itemData.ItemSprite;
        if (itemNameText != null)
            itemNameText.text = itemData.ItemName;
        if (itemEffectText != null)
            itemEffectText.text = itemData.ItemExplain; //효과 요약
        if (itemDescriptionText != null)
            itemDescriptionText.text = itemData.ItemDescription; //상세 설명
    }

    //설명창 닫기
    public void ClosePanel()
    {
        explainPanel.SetActive(false);
        isPanelActive = false;
    }
}
