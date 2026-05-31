using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public struct BuffSpriteEntry
{
    public BuffType buffType;
    public Sprite sprite;
}

public class BuffIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("이미지 컴포넌트")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Image baseImage;
    [SerializeField] private TextMeshProUGUI buffStackText;

    [Header("버프 종류별 스프라이트")]
    [SerializeField] private List<BuffSpriteEntry> buffSpriteEntries = new List<BuffSpriteEntry>();

    private Dictionary<BuffType, Sprite> buffSpriteDictionary = new Dictionary<BuffType, Sprite>();
    private Buff buffData;
    private BuffToolTipUI tooltip;

    private void Awake()
    {
        foreach (BuffSpriteEntry entry in buffSpriteEntries)
        {
            if (!buffSpriteDictionary.ContainsKey(entry.buffType))
            {
                buffSpriteDictionary.Add(entry.buffType, entry.sprite);
            }
        }
    }

    private void OnDisable()
    {
        if (tooltip != null && tooltip.gameObject.activeSelf)
            tooltip.Hide(); // 직접 비활성화 대신 캡슐화된 메서드 사용
    }

    private void Update()
    {
        UpdateUI();
    }

    public void Initialize(Buff newBuffData)
    {
        buffData = newBuffData;
        Debug.Log($"[BuffIconUI] Initialize 호출됨: {buffData.BuffType}");
        SetSprites();

        iconImage.fillAmount = 1f;
        buffData.InitialDuration = buffData.CurrentDuration;

        baseImage.color = buffData.IsDebuff ? Color.red : Color.green;

        // [수정/개선] GC 부하를 일으키던 ToString().StartsWith()를 헬퍼 클래스로 이관
        if (BuffUIDataHelper.IsPotionBuff(buffData.BuffType))
        {
            iconImage.color = new Color(1f, 1f, 1f, 0.6f); // 반투명 표시
            cooldownOverlay.color = new Color(1f, 1f, 1f, 0.6f);
        }
    }

    public void UpdateData(Buff newBuffData)
    {
        buffData = newBuffData;
        UpdateUI();
    }

    private void SetSprites()
    {
        if (buffData == null || !buffSpriteDictionary.ContainsKey(buffData.BuffType)) return;

        Sprite sprite = buffSpriteDictionary[buffData.BuffType];

        if (iconImage != null)
        {
            iconImage.sprite = sprite;
        }
    }

    private void UpdateUI()
    {
        if (buffData == null) return;

        float fill = Mathf.Clamp01(1f - (buffData.CurrentDuration / buffData.InitialDuration));
        
        buffStackText.text = buffData.CurrentBuffLevel == 0 ? "" : (buffData.CurrentBuffLevel + 1).ToString();
        cooldownOverlay.fillAmount = fill;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            // 디버깅용 메서드(ShowBuffDetail) 호출 제거하여 깔끔하게 정리
            tooltip.ShowTooltip(buffData, iconImage.sprite);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null && tooltip.IsVisible())
        {
            tooltip.Hide();
        }
    }

    public void SetTooltipReference(BuffToolTipUI tooltipUI)
    {
        tooltip = tooltipUI;
    }
}